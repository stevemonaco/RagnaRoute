using System;
using System.Collections.Generic;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using FluentAvalonia.UI.Controls;

namespace RagnaRoute.ViewExtenders;

/// <summary>
/// Template selector that is specific to FluentAvalonia's NavigationView
/// </summary>
internal class MenuTemplateSelector : DataTemplateSelector
{
    [Content]
    public Dictionary<Type, IDataTemplate> Templates { get; } = new();

    protected override IDataTemplate SelectTemplateCore(object item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        var type = item.GetType();

        if (Templates.TryGetValue(type, out var template))
            return template;
        else
            throw new ArgumentException(nameof(item));
    }
}
