using Antlr;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

public class JSONDeserializer {
	public static object Deserialize( string json ) {
		JSONRecognizerTree tree;
		
		try {
			ANTLRStringStream input = new ANTLRStringStream( json );
        	JSONRecognizerLexer lexer = new JSONRecognizerLexer( input );
        	CommonTokenStream tokens = new CommonTokenStream( lexer );
        	JSONRecognizerParser parser = new JSONRecognizerParser( tokens );
		
			CommonTree t = parser.ToTree();
			CommonTreeNodeStream nodes = new CommonTreeNodeStream( t );
			tree = new JSONRecognizerTree( nodes );
		}
		catch( System.Exception ex ) {
			UnityEngine.Debug.LogError( ex );
			UnityEngine.Debug.LogError( ex.StackTrace );
			return "";
		}
		
		return tree.Deserialize();
	}
}