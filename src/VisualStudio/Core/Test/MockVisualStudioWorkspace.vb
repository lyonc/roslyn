' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Editor.UnitTests.Workspaces
Imports Microsoft.CodeAnalysis.FindSymbols
Imports Microsoft.VisualStudio.LanguageServices.Implementation.Library.ObjectBrowser.Lists
Imports Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem

Namespace Microsoft.VisualStudio.LanguageServices.UnitTests
    Friend Class MockVisualStudioWorkspace
        Inherits VisualStudioWorkspace

        Private ReadOnly _workspace As TestWorkspace
        Private ReadOnly _fileCodeModels As New Dictionary(Of DocumentId, EnvDTE.FileCodeModel)

        Public Sub New(workspace As TestWorkspace)
            MyBase.New(workspace.Services.HostServices, backgroundWork:=WorkspaceBackgroundWork.ParseAndCompile)

            _workspace = workspace
            SetCurrentSolution(workspace.CurrentSolution)
        End Sub

        Public Overrides Function CanApplyChange(feature As ApplyChangesKind) As Boolean
            Return _workspace.CanApplyChange(feature)
        End Function

        Protected Overrides Sub OnDocumentTextChanged(document As Document)
            Assert.True(_workspace.TryApplyChanges(_workspace.CurrentSolution.WithDocumentText(document.Id, document.GetTextAsync().Result)))
            SetCurrentSolution(_workspace.CurrentSolution)
        End Sub

        Public Overrides Sub CloseDocument(documentId As DocumentId)
            _workspace.CloseDocument(documentId)
            SetCurrentSolution(_workspace.CurrentSolution)
        End Sub

        Protected Overrides Sub ApplyDocumentRemoved(documentId As DocumentId)
            Assert.True(_workspace.TryApplyChanges(_workspace.CurrentSolution.RemoveDocument(documentId)))
            SetCurrentSolution(_workspace.CurrentSolution)
        End Sub

        Public Overrides Function GetFilePath(documentId As DocumentId) As String
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetHierarchy(projectId As ProjectId) As Microsoft.VisualStudio.Shell.Interop.IVsHierarchy
            Return Nothing
        End Function

        Friend Overrides Function OpenInvisibleEditor(documentId As DocumentId) As IInvisibleEditor
            Return New MockInvisibleEditor(documentId, _workspace)
        End Function

        Friend Overrides Function OpenInvisibleEditor(document As IVisualStudioHostDocument) As IInvisibleEditor
            Return New MockInvisibleEditor(document.Id, _workspace)
        End Function

        Public Overrides Function GetFileCodeModel(documentId As DocumentId) As EnvDTE.FileCodeModel
            Return _fileCodeModels(documentId)
        End Function

        Public Overrides Function TryGoToDefinition(symbol As ISymbol, project As Project, cancellationToken As CancellationToken) As Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function TryFindAllReferences(symbol As ISymbol, project As Project, cancellationToken As CancellationToken) As Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Sub DisplayReferencedSymbols(solution As Solution, referencedSymbols As IEnumerable(Of ReferencedSymbol))
            Throw New NotImplementedException()
        End Sub

        Friend Overrides Function GetBrowseObject(symbolListItem As SymbolListItem) As Object
            Throw New NotImplementedException()
        End Function

        Friend Sub SetFileCodeModel(id As DocumentId, codeModel As EnvDTE.FileCodeModel)
            _fileCodeModels.Add(id, codeModel)
        End Sub

        Friend Overrides Function RenameFileCodeModelInstance(documentId As DocumentId, newFilePath As String) As Boolean
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class MockInvisibleEditor
        Implements IInvisibleEditor

        Private ReadOnly _documentId As DocumentId
        Private ReadOnly _workspace As TestWorkspace

        Public Sub New(documentId As DocumentId, workspace As TestWorkspace)
            Me._documentId = documentId
            Me._workspace = workspace
        End Sub

        Public ReadOnly Property TextBuffer As Global.Microsoft.VisualStudio.Text.ITextBuffer Implements IInvisibleEditor.TextBuffer
            Get
                Return Me._workspace.GetTestDocument(Me._documentId).GetTextBuffer()
            End Get
        End Property

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub

    End Class
End Namespace
