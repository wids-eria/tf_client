using System.Net;

public class NetworkUtils {
	public static string	DetermineLocalAddress() {
		IPHostEntry host;
		string localIP = "?";
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList) {
    		if (ip.AddressFamily.ToString() == "InterNetwork") {
        		localIP = ip.ToString();
  			}
		}
		return localIP;
	}
}