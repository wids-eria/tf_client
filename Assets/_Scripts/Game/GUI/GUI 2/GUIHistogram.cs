/* *****************************************************************************
 * 
 *								EDUCATION RESEARCH GROUP
 *							MORGRIDGE INSTITUTE FOR RESEARCH
 * 			
 * 				
 * Copyright (c) 2012 EDUCATION RESEARCH, MORGRIDGE INSTITUTE FOR RESEARCH
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated  * documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 *  
 * 
 ******************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIHistogramSeries<XValueType, YValueType>
{
	private Dictionary<XValueType, YValueType> Points;
	
	public XValueType XMin {
		get { return xMin; }
		set { xMin = value; }
	}
	
	private XValueType xMin;
	private XValueType xMax;
	private YValueType yMin;
	private YValueType yMax; 
	
	public GUIHistogramSeries()
	{
		Points = new Dictionary<XValueType, YValueType>();
	}
	
	public void AddPoint(XValueType x, YValueType y)
	{
		/*Points[x] = y;	
		if(xMin != null)
		{
			if(IsXLessThan<XValueType>(x, xMin))
			{
				xMin = x;	
			}
			
			if(IsXGreaterThan<XValueType>(x, xMax))
			{
				xMax = x;	
			}
			
			if(IsYLessThan(y,yMin))
			{
				yMin = y;
			}
			
			if(IsYGreaterThan(y,yMax))
			{
				yMax = y;	
			}
		}
		else
		{
			//If one is null, they are all null
			xMin = x;	
			xMax = x;
			yMin = y;
			yMax = y;
		}*/
	}
	
	private bool IsXGreaterThan<XValueType>(XValueType value, XValueType other) where XValueType : IComparable
	{
	    return value.CompareTo(other) > 0;
	}
	
	private static bool IsXLessThan<XValueType>(XValueType value, XValueType other) where XValueType : IComparable
	{
	    return value.CompareTo(other) < 0;
	}
	
	private bool IsYGreaterThan<YValueType>(YValueType value, YValueType other) where YValueType : IComparable
	{
	    return value.CompareTo(other) > 0;
	}
	
	private static bool IsYLessThan<YValueType>(YValueType value, YValueType other) where YValueType : IComparable
	{
	    return value.CompareTo(other) < 0;
	}
}

[System.Serializable]   
public class GUIHistogram<XValueType, YValueType> : GUIObject 
{
	protected readonly float m_histogramHeight = 100f;
	protected Rect m_histogramRect;
	
	private List<GUIHistogramSeries<XValueType, YValueType>> series;
	private XValueType xMin;
	private XValueType xMax;
	private YValueType yMin;
	private YValueType yMax;
	
	public GUIHistogram(Rect zoneSize) : base(zoneSize) 
	{
		series = new List<GUIHistogramSeries<XValueType, YValueType>>();
		m_histogramRect = zoneSize;
	}
		
	public void AddSeries(GUIHistogramSeries<XValueType, YValueType> newSeries)
	{
		series.Add(newSeries);	
		if(xMin != null)
		{
			
		}
		else
		{
			
		}
	}
	
	public override void Draw()
	{
		if (!visible) return;
		DisplayHistogram();	
	}
	
	public void DisplayHistogram()
	{
		
	}
}
