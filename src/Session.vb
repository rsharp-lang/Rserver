Imports System.IO
Imports Flute.Http.Configurations
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime

<Package("session")>
Module Session

    Dim ssid As String
    Dim config As Configuration

    <ExportAPI("load")>
    Public Function load(Optional env As Environment = Nothing) As String
        Dim std_in As String = New StreamReader(Console.OpenStandardInput).ReadToEnd
        Dim cookie_str As String = env.globalEnvironment.options.getOption("cookies")
        Dim config_str As String = env.globalEnvironment.options.getOption("configs")
        Dim cookies As Cookies = Cookies.ParseCookies(cookie_str)
        Dim ssid As String = cookies.GetCookie("session_id")
        Session.ssid = ssid
        Session.config = config_str.LoadJSON(Of Configuration)
        Return ssid
    End Function

    <ExportAPI("session_id")>
    Public Function session_id() As String
        Return ssid
    End Function

    <ExportAPI("get_string")>
    Public Function get_string(key As String) As String
        Return Flute.SessionManager.Open(key, config).OpenKeyString(key)
    End Function

    <ExportAPI("get_number")>
    Public Function get_number(key As String) As Double
        Return Flute.SessionManager.Open(key, config).OpenKeyDouble(key)
    End Function

    <ExportAPI("get_integer")>
    Public Function get_integer(key As String) As Integer
        Return Flute.SessionManager.Open(key, config).OpenKeyInteger(key)
    End Function

End Module
