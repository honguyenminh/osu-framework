// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input.Events;
using osu.Framework.Extensions.EnumExtensions;

namespace osu.Framework.Graphics.UserInterface
{
    public abstract class DirectorySelectorDirectory : DirectorySelectorItem
    {
        protected readonly DirectoryInfo Directory;
        protected override string FallbackName => Directory.Name;

        [Resolved]
        private Bindable<DirectoryInfo> currentDirectory { get; set; }

        protected DirectorySelectorDirectory(DirectoryInfo directory, string displayName = null)
            : base(displayName)
        {
            Directory = directory;

            try
            {
                bool isHidden = directory?.Attributes.HasFlagFast(FileAttributes.Hidden) == true;

                // System drives show up as `System | Hidden | Directory` but shouldn't be shown in a hidden state.
                bool isSystem = directory?.Attributes.HasFlagFast(FileAttributes.System) == true;

                if (isHidden && !isSystem)
                    ApplyHiddenState();
            }
            catch (UnauthorizedAccessException)
            {
                // checking attributes on access-controlled directories will throw an error so we handle it here to prevent a crash
            }
        }

        protected override bool OnClick(ClickEvent e)
        {
            currentDirectory.Value = Directory;
            return true;
        }
    }
}
