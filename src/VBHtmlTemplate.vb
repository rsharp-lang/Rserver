Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.WebCloud.VBScript

''' <summary>
''' vbhtml template render/compiler
''' </summary>
<Package("vbhtml")>
Module VBHtmlTemplate

    <ExportAPI("rendering")>
    Public Function Rendering(file As String, wwwroot As String,
                              <RListObjectArgument>
                              Optional symbols As list = Nothing,
                              Optional env As Environment = Nothing) As Object

        Dim vars As Dictionary(Of String, Object) = symbols.AsGeneric(Of Object)(env, [default]:="")
        Dim html As String = vbhtml.ReadHTML(wwwroot, file, vars)

        Return html
    End Function

    ''' <summary>
    ''' compile the template file as single html file for publish release
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="wwwroot"></param>
    ''' <returns></returns>
    <ExportAPI("compile")>
    Public Function compile(file As String, wwwroot As String) As Object
        Return vbhtml.ReadHTML(wwwroot, file)
    End Function
End Module
