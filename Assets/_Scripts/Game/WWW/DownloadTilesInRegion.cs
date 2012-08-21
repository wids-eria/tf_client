#define FOR_JONGEE


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RegionTiles {
	public int 	NumCols;
	public int 	NumRows;
	protected Rect	RegionRect;
	protected Size	TileSize;
	
	public RegionTiles( int numRows, int numCols ) {
		NumCols = numCols;
		NumRows = numRows;
	}
	
	public void		RebuildTileSize( Region region ) {
		RegionRect = new Rect();
		RegionRect.xMin = region.left;
		RegionRect.xMax = region.right;
		RegionRect.yMin = region.top;
		RegionRect.yMax = region.bottom;
		TileSize = new Size( (region.width - 1.0f) / (float)NumCols, (region.height - 1.0f) / (float)NumRows );
	}
	
	public void		DetermineTileRect( int xIndex, int yIndex, out Rect rect ) {
		ExceptionHelper.Throw<ArgumentOutOfRangeException>( xIndex >= 0 && xIndex < NumCols );
		ExceptionHelper.Throw<ArgumentOutOfRangeException>( yIndex >= 0 && yIndex < NumRows );
		
		//parameters.Add("x_min", region.left.ToString());
		//parameters.Add("x_max", region.right.ToString());
		//parameters.Add("y_min", Megatile.ToServerY(region.top).ToString());
		//parameters.Add("y_max", Megatile.ToServerY(region.bottom).ToString());
		
		rect = new Rect();
		rect.xMin = RegionRect.xMin + xIndex * TileSize.width;
		Debug.Log( string.Format("xIndex: {0}, TileSize.width: {1}, RegionRect.xMax: {2}", xIndex, TileSize.width, RegionRect.xMax) );
		//ExceptionHelper.Throw<ArgumentOutOfRangeException>( rect.xMin == RegionRect.xMin );
		
		rect.xMax = rect.xMin + TileSize.width;
		//Debug.Log( string.Format("xIndex: {0}, TileSize.width: {1}, RegionRect.xMax: {1}", xIndex, TileSize.width, RegionRect.xMax) );
		//ExceptionHelper.Throw<ArgumentOutOfRangeException>( rect.xMax == RegionRect.xMax );
		
		rect.yMin = RegionRect.yMin - yIndex * TileSize.height;
		Debug.Log( string.Format("yIndex: {0}, TileSize.height: {1}, RegionRect.yMax: {1}", yIndex, TileSize.height, RegionRect.yMax) );
		//ExceptionHelper.Throw<ArgumentOutOfRangeException>( rect.yMin == RegionRect.yMin );
		
		rect.yMax = rect.yMin - TileSize.height;
		//Debug.Log( string.Format("rect.yMax: {0}, RegionRect.yMax: {1}", rect.yMax, RegionRect.yMax) );
		//ExceptionHelper.Throw<ArgumentOutOfRangeException>( rect.yMax == RegionRect.yMax );
	}
}

////////////

public class DownloadRegionTileCoroutine : WebCoroutine<Rect, Region, System.DateTime> {
	public Megatile[] 	Tiles;
	
	public DownloadRegionTileCoroutine() {
		AddExecutionHandler( OnExecuteStage1 );
		AddExecutionHandler( OnExecuteStage2 );
	}
	
	protected void AddRectToParameters( Rect rect, HTTP.Request request ) {				
		request.AddParameter( "x_min", rect.xMin.ToString() );
		request.AddParameter( "x_max", rect.xMax.ToString() );
		request.AddParameter( "y_min", Megatile.ToServerY((int)rect.yMin).ToString() );
		request.AddParameter( "y_max", Megatile.ToServerY((int)rect.yMax).ToString() );
	}
	
	private string toRFC2822(string dateString)
	{
			
		return dateString.Substring(0,dateString.Length -4) + " +0000";	
	}
	
	protected IEnumerator	OnExecuteStage1( Rect rect, Region region, System.DateTime lastUpdate, AWebCoroutine self ) {
		self.Request = new HTTP.Request( "Get", WebRequests.urlGetMegatiles );
		
		Debug.Log ("Sending request with timestamp: " + toRFC2822(lastUpdate.ToString ("r")));
		self.Request.AddHeader("If-Modified-Since", toRFC2822(lastUpdate.ToString ("r")));
		
		region.EncapsulateMegatiles();
		AddRectToParameters( rect, self.Request );
		self.Request.AddParameters( WebRequests.authenticatedParameters );
				
		self.Request.Send();
		
		while( !self.RequestIsDone ) {
			yield return 0;
		}
	}
	
	protected IEnumerator	OnExecuteStage2( Rect rect, Region region, System.DateTime lastUpdate, AWebCoroutine self ) {
		Debug.Log (self.ResponseText);
		Func<string, Megatiles>	asyncDelegate = JSONDecoder.Decode<Megatiles>;
		IAsyncResult ar = asyncDelegate.BeginInvoke( self.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		Tiles = asyncDelegate.EndInvoke( ar ).megatiles;
		yield break;
	}
}

////////////

public class DownloadTilesInRegionCoroutine : WebCoroutine<Region, bool> 
{
	protected List< ResourceTile >					resourceTiles = new List<ResourceTile>();
	protected RegionTiles							regionTiles;
	protected List< DownloadRegionTileCoroutine >	regionTileCoroutines = new List<DownloadRegionTileCoroutine>();
	protected MonoBehaviour							owner;
	protected System.DateTime						lastUpdateTime;
	protected Region								previousRegion;
		
	public override bool		IsDone {
		get {
			return TilesAreDownloaded();
		}
	}
	
	public DownloadTilesInRegionCoroutine( MonoBehaviour owner ) {
		regionTiles = new RegionTiles( 1, 1 );
		this.owner = owner;
		
		AddExecutionHandler( OnExecuteStage1 );
		AddExecutionHandler( OnExecuteStage2 );
	}
	
	protected override void	OnStart() {
		base.OnStart();
		SignalSystemBusy();
	}
	
	protected override void	BroadcastOnComplete() {
		base.BroadcastOnComplete();
		
		SignalSystemIdle();
	}
	
	protected bool				TilesAreDownloaded() {
		foreach( DownloadRegionTileCoroutine co in regionTileCoroutines ) {
			if( !co.IsDone ) {
				return false;
			}
		}
		
		return true;
	}
		
	protected IEnumerator OnExecuteStage1( Region region, bool updateAll, AWebCoroutine self ) {
		Rect rect;
		DownloadRegionTileCoroutine	co;
		Region downloadRegion = region;
		
		// Wait a fram	e to see if size is updated
		// Assumption is that we are waiting for another WWW transaction to finish
		while( Megatile.size <= 0 ) {
			yield return 0;
		}
					
		/*if(!updateAll)
		{
			if(previousRegion.left != null)
			{
				downloadRegion = region.GetOverlap(previousRegion);
			}
			else
			{
				Debug.Log ("init prev region");
				previousRegion = region;
				downloadRegion = region;
			}
		}*/
		
		regionTiles.RebuildTileSize( downloadRegion );
		
		for( int ix = 0; ix < regionTiles.NumCols; ++ix ) {
			for( int iy = 0; iy < regionTiles.NumRows; ++iy ) {
				regionTiles.DetermineTileRect( ix, iy, out rect );
				
				co = new DownloadRegionTileCoroutine();
				if(updateAll)
					co.Start( owner, rect, downloadRegion, System.DateTime.MinValue );
				else
					co.Start( owner, rect, downloadRegion, lastUpdateTime );					
				regionTileCoroutines.Add( co );
				yield return 0;
			}
		}
		
		while( !TilesAreDownloaded() ) {
			yield return 0;
		}
	}
	
	protected IEnumerator OnExecuteStage2( Region region, bool updateAll, AWebCoroutine self ) {
		List<ResourceTile> resourceTiles = new List<ResourceTile>();
		
		foreach( DownloadRegionTileCoroutine co in regionTileCoroutines ) 
		{
			if(co.Tiles != null)
			{
				foreach( Megatile megatile in co.Tiles ) 
				{
					System.DateTime temp = System.DateTime.Parse(megatile.updated_at);
					if(System.DateTime.Compare(lastUpdateTime,temp) < 0)
					{
						lastUpdateTime = temp.Add(new System.TimeSpan(7,0,0));
					}
	
					foreach(ResourceTile t in megatile.resource_tiles)
					{
						//HACK HACK HACK THIS IS
						if (megatile.owner != null) {
							t.idOwner = megatile.owner.id;
						}
						t.idMegatile = megatile.id;
					}
					resourceTiles.AddRange( megatile.resource_tiles );
				}
			}
			yield return 0;
		}
		
		Debug.Log("Last Updated Tile:" + lastUpdateTime.ToString("r"));
		TerrainManager.use.lastUpdateTime = lastUpdateTime;
		TerrainManager.use.SetTerrainFromResourceTiles( resourceTiles.ToArray(), SetTerrainMode.Flush );
		yield break;
	}
}