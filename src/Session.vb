#Region "Microsoft.VisualBasic::fc1f8d0c5b160745147ce59521a835ee, win32_desktop\src\Rstudio\Rserver\src\Session.vb"

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


    ' Code Statistics:

    '   Total Lines: 57
    '    Code Lines: 43 (75.44%)
    ' Comment Lines: 3 (5.26%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (19.30%)
    '     File Size: 1.83 KB


    ' Module Session
    ' 
    '     Function: get_integer, get_number, get_string, load, session_id
    '               set_string
    ' 
    ' /********************************************************************************/

#End Region

Imports Flute.Http.Configurations
Imports Flute.SessionManager
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime

''' <summary>
''' session data access
''' </summary>
<Package("session")>
Module Session

    Dim ssid As String
    Dim config As Configuration
    Dim file As SessionFile

    <ExportAPI("load")>
    Public Function load(Optional env As Environment = Nothing) As String
        Dim cookie_str As String = System.Environment.GetEnvironmentVariable("cookies")
        Dim config_str As String = System.Environment.GetEnvironmentVariable("configs")
        Dim cookies As Dictionary(Of String, String) = cookie_str.LoadJSON(Of Dictionary(Of String, String))
        Dim ssid As String = cookies.TryGetValue("session_id")

        Session.ssid = ssid
        Session.config = config_str.LoadJSON(Of Configuration)
        Session.file = Flute.SessionManager.Open(ssid, config)

        Return ssid
    End Function

    <ExportAPI("session_id")>
    Public Function session_id() As String
        Return ssid
    End Function

    <ExportAPI("get_string")>
    Public Function get_string(key As String) As String
        Return file.OpenKeyString(key)
    End Function

    <ExportAPI("get_number")>
    Public Function get_number(key As String) As Double
        Return file.OpenKeyDouble(key)
    End Function

    <ExportAPI("get_integer")>
    Public Function get_integer(key As String) As Integer
        Return file.OpenKeyInteger(key)
    End Function

    <ExportAPI("set_string")>
    Public Function set_string(key As String, value As String) As Object
        Return file.SaveKey(key, value)
    End Function

End Module

