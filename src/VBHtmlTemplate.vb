#Region "Microsoft.VisualBasic::49a86298deff7182780593e472ebdaca, G:/GCModeller/src/workbench/win32_desktop/src/Rstudio/Rserver/src//VBHtmlTemplate.vb"

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

'   Total Lines: 36
'    Code Lines: 22
' Comment Lines: 9
'   Blank Lines: 5
'     File Size: 1.28 KB


' Module VBHtmlTemplate
' 
'     Function: compile, Rendering
' 
' /********************************************************************************/

#End Region

Imports Flute.Template
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' vbhtml template render/compiler
''' </summary>
<Package("vbhtml")>
Module VBHtmlTemplate

    <ExportAPI("rendering")>
    Public Function Rendering(file As String,
                              <RListObjectArgument>
                              Optional symbols As list = Nothing,
                              Optional env As Environment = Nothing) As Object

        Dim vars As Dictionary(Of String, Object) = symbols.AsGeneric(Of Object)(env, [default]:="")
        Dim html As String = VBHtml.ReadHTML(file, vars)

        Return html
    End Function

    ''' <summary>
    ''' compile the template file as single html file for publish release
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("compile")>
    Public Function compile(file As String) As Object
        Return VBHtml.ReadHTML(file)
    End Function
End Module

