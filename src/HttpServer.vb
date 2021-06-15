#Region "Microsoft.VisualBasic::d5c0bc2092065c59184e7491186b8eff, studio\Rsharp_kit\webKit\HttpServer.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

' Module HttpServer
' 
'     Function: serve
' 
' 
' /********************************************************************************/

#End Region

#If netcore5 = 0 Then

Imports System.Threading
Imports Flute.Http
Imports Flute.Http.Core
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components.Interface
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("http", Category:=APICategories.UtilityTools)>
Public Module HttpServer

    <ExportAPI("http_socket")>
    Public Function createDriver(Optional silent As Boolean = True) As HttpDriver
        Return New HttpDriver(silent)
    End Function

    <ExportAPI("getHeaders")>
    Public Function getHeaders(req As HttpRequest) As list
        Return New list(RType.GetRSharpType(GetType(String))) With {
            .slots = req.HttpHeaders _
                .ToDictionary(Function(h) h.Key,
                              Function(h)
                                  Return CObj(h.Value)
                              End Function)
        }
    End Function

    <ExportAPI("getUrl")>
    Public Function getUrl(req As HttpRequest) As list
        Dim url As URL = req.URL
        Dim queryData As New list(RType.GetRSharpType(GetType(String))) With {
            .slots = url.query _
                .ToDictionary(Function(q) q.Key,
                              Function(q)
                                  Return CObj(q.Value)
                              End Function)
        }

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"url", url.ToString},
                {"path", url.path},
                {"hostName", url.hostName},
                {"hash", url.hashcode},
                {"query", queryData},
                {"port", url.port},
                {"protocol", url.protocol}
            }
        }
    End Function

    ''' <summary>
    ''' add custom http response headers
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="headers"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("headers")>
    Public Function customResponseHeader(driver As HttpDriver, <RListObjectArgument> headers As list, Optional env As Environment = Nothing) As HttpDriver
        For Each header As KeyValuePair(Of String, String) In headers.AsGeneric(Of String)(env)
            Call driver.AddResponseHeader(header.Key, header.Value)
        Next

        Return driver
    End Function

    <ExportAPI("listen")>
    Public Function serve(driver As HttpDriver, Optional port% = -1, Optional env As Environment = Nothing) As Integer
        Dim httpPort As Integer = If(port <= 0, Rnd() * 30000, port)
        Dim socket As HttpSocket = driver.GetSocket(httpPort)
        Dim localUrl$ = $"http://localhost:{httpPort}/"

        If env.globalEnvironment.debugMode Then
            Call New Thread(
                Sub()
                    Call Thread.Sleep(3000)
                    Call Process.Start(localUrl)
                End Sub) _
 _
            .Start()
        End If

        Return socket.Run()
    End Function

    <ExportAPI("httpMethod")>
    Public Function httpMethod(driver As HttpDriver, method As String, process As RFunction, Optional env As Environment = Nothing) As HttpDriver
        Call driver.HttpMethod(
            method:=method.ToUpper,
            handler:=Sub(req, response)
                         Call process.Invoke({req, response}, env)
                     End Sub)

        Return driver
    End Function
End Module
#End If
