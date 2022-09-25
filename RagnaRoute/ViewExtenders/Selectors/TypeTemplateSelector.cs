using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using RagnaRoute.ViewModels;

namespace RagnaRoute.ViewExtenders;
internal class TypeTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<Type, IDataTemplate> Templates { get; } = new();

    public IControl Build(object? param)
    {
        if (param is Type key)
        {
            var control = Templates[key].Build(param);
            if (control is not null)
                return control;
            else
                return new TextBlock { Text = $"Template not found: {key.FullName}" };

        }
        else
            throw new ArgumentException(nameof(param));
    }

    public bool Match(object? data)
    {
        return data is INavigationChild;
    }
}
