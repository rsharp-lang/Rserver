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

Imports Flute.Http
Imports Flute.Http.Core
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("http", Category:=APICategories.UtilityTools)>
Public Module HttpServer

    <ExportAPI("http_socket")>
    Public Function createDriver() As HttpDriver
        Return New HttpDriver
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
    Public Function serve(driver As HttpDriver, Optional port% = -1, Optional env As Environment = Nothing) As HttpSocket
        Dim httpPort As Integer = If(port <= 0, Rnd() * 30000, port)
        Dim socket As HttpSocket = driver.GetSocket(httpPort)
        Dim localUrl$ = $"http://localhost:{httpPort}/"

        Call socket.DriverRun

        If env.globalEnvironment.debugMode Then
            Call Process.Start(localUrl)
        End If

        Return socket
    End Function
End Module
#End If
