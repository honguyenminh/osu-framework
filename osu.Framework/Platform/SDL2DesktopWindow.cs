// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using SDL2;

namespace osu.Framework.Platform
{
    internal class SDL2DesktopWindow : SDL2Window
    {
        public SDL2DesktopWindow(GraphicsSurfaceType surfaceType)
            : base(surfaceType)
        {
        }

        /// <summary>
        /// Attempts to flash the window in order to request the user's attention while unfocused.
        /// <para>
        /// The flash can be canceled with <see cref="CancelFlash"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This behaviour is available only on desktop platforms and may differ depending on the operating system.
        /// </remarks>
        /// <param name="untilFocused">Whether the window should flash briefly or until focused.</param>
        public void Flash(bool untilFocused = false) => ScheduleCommand(() =>
        {
            if (IsActive.Value)
                return;

            SDL.SDL_FlashWindow(SDLWindowHandle, untilFocused
                ? SDL.SDL_FlashOperation.SDL_FLASH_UNTIL_FOCUSED
                : SDL.SDL_FlashOperation.SDL_FLASH_BRIEFLY);
        });

        /// <summary>
        /// Cancels any flash triggered with <see cref="Flash"/>
        /// </summary>
        /// <remarks>
        /// This can also cancel brief flashes (especially on Linux and Windows).
        /// </remarks>
        public void CancelFlash()
            => ScheduleCommand(() => SDL.SDL_FlashWindow(SDLWindowHandle, SDL.SDL_FlashOperation.SDL_FLASH_CANCEL));

        protected override void UpdateWindowStateAndSize(WindowState state, Display display, DisplayMode displayMode)
        {
            // this reset is required even on changing from one fullscreen resolution to another.
            // if it is not included, the GL context will not get the correct size.
            // this is mentioned by multiple sources as an SDL issue, which seems to resolve by similar means (see https://discourse.libsdl.org/t/sdl-setwindowsize-does-not-work-in-fullscreen/20711/4).
            SDL.SDL_SetWindowBordered(SDLWindowHandle, SDL.SDL_bool.SDL_TRUE);
            SDL.SDL_SetWindowFullscreen(SDLWindowHandle, (uint)SDL.SDL_bool.SDL_FALSE);
            SDL.SDL_RestoreWindow(SDLWindowHandle);

            base.UpdateWindowStateAndSize(state, display, displayMode);
        }
    }
}
