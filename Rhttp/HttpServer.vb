﻿#Region "Microsoft.VisualBasic::d5c0bc2092065c59184e7491186b8eff, studio\Rsharp_kit\webKit\HttpServer.vb"

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

Imports Flute.Http.Core
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime

<Package("http.socket", Category:=APICategories.UtilityTools)>
Public Module HttpServer

    <ExportAPI("serve")>
    Public Function serve(content$, Optional port% = -1, Optional env As Environment = Nothing) As HttpSocket
        Dim httpPort As Integer = If(port <= 0, Rnd() * 30000, port)
        Dim socket As New HttpSocket(
            app:=Sub(req, rep) Call rep.WriteHTML(content),
            threads:=1,
            port:=httpPort
        )
        Dim localUrl$ = $"http://localhost:{socket.localPort}/"

        Call socket.DriverRun

        If env.globalEnvironment.debugMode Then
            Call Process.Start(localUrl)
        End If

        Return socket
    End Function
End Module
#End If
