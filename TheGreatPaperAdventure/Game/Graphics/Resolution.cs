//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TGPA.Game.Graphics
{
    /// <summary>
    /// Easy resolution management
    /// </summary>
    /// <remarks>Author : David Gouveia</remarks>
    static class Resolution
    {
        public static void Initialize(GraphicsDeviceManager graphics, int deviceWidth, int deviceHeight, int virtualWidth, int virtualHeight, bool fullscreen)
        {
            _graphics = graphics;

            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;

            SetResolution(deviceWidth, deviceHeight, fullscreen);
        }

        public static void SetResolution(int width, int height, bool fullscreen)
        {
            DeviceWidth = width;
            DeviceHeight = height;

            _isFullScreen = fullscreen;

            ApplyChanges();
            CalculateViewport();

            _dirty = true;
        }

        public static int DeviceWidth { get; private set; }

        public static int DeviceHeight { get; private set; }

        public static int VirtualWidth { get; private set; }

        public static int VirtualHeight { get; private set; }

        public static int ViewportX { get; private set; }
        public static int ViewportY { get; private set; }
        public static int ViewportWidth { get; private set; }
        public static int ViewportHeight { get; private set; }

        public static Matrix ResolutionMatrix
        {
            get
            {
                if (_dirty)
                {
                    Invalidate();
                    _dirty = false;
                }

                return _resolutionMatrix;
            }
        }

        private static void Invalidate()
        {
            _resolutionMatrix = Matrix.CreateScale(
                           (float)_graphics.GraphicsDevice.Viewport.Width / VirtualWidth,
                           (float)_graphics.GraphicsDevice.Viewport.Width / VirtualWidth,
                           1f);
        }

        private static void ApplyChanges()
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (_isFullScreen == false)
            {
                if ((DeviceWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) && (DeviceHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    _graphics.PreferredBackBufferWidth = DeviceWidth;
                    _graphics.PreferredBackBufferHeight = DeviceHeight;
                    _graphics.IsFullScreen = false;
                    _graphics.ApplyChanges();
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate through the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == DeviceWidth) && (dm.Height == DeviceHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        _graphics.PreferredBackBufferWidth = DeviceWidth;
                        _graphics.PreferredBackBufferHeight = DeviceHeight;
                        _graphics.IsFullScreen = true;
                        _graphics.ApplyChanges();
                    }
                }
            }

            _dirty = true;

            DeviceWidth = _graphics.PreferredBackBufferWidth;
            DeviceHeight = _graphics.PreferredBackBufferHeight;
        }

        private static void CalculateViewport()
        {
            float targetAspectRatio = (float)VirtualWidth / VirtualHeight;

            // Try letterbox
            int width = _graphics.PreferredBackBufferWidth;
            int height = (int)((width / targetAspectRatio) + .5f);

            // If it doesn't fit then use pillarbox
            if (height > _graphics.PreferredBackBufferHeight)
            {
                height = _graphics.PreferredBackBufferHeight;
                width = (int)((height * targetAspectRatio) + .5f);
            }

            Viewport viewport = new Viewport
            {
                X = (_graphics.PreferredBackBufferWidth / 2) - (width / 2),
                Y = (_graphics.PreferredBackBufferHeight / 2) - (height / 2),
                Width = width,
                Height = height,
                MinDepth = 0,
                MaxDepth = 1
            };

            ViewportX = viewport.X;
            ViewportY = viewport.Y;
            ViewportWidth = (width > 1) ? viewport.Width : DeviceWidth;
            ViewportHeight = (height > 1) ? viewport.Height : DeviceHeight;

            _graphics.GraphicsDevice.Viewport = viewport;

            _dirty = true;
            
#if XBOX
            Viewport tempViewport = new Viewport(0, 0, DeviceWidth, DeviceHeight);
            Rectangle temp = tempViewport.TitleSafeArea;
            // nondidju ! On va pas se limiter à un title safe area calibré sans bandes noires ! Let's correct this sh*t. 
            if (Resolution.ViewportX >= tempViewport.TitleSafeArea.Left) temp.X = 0;
            else temp.X = tempViewport.TitleSafeArea.Left - Resolution.ViewportX;
            if (Resolution.ViewportY >= tempViewport.TitleSafeArea.Top) temp.Y = 0;
            else temp.Y = tempViewport.TitleSafeArea.Top - Resolution.ViewportY;

            if (Resolution.ViewportX + Resolution.ViewportWidth <= tempViewport.TitleSafeArea.X + tempViewport.Width) temp.Width = 1024 - temp.Left;
            else temp.Width = 1024 - (DeviceWidth - (tempViewport.TitleSafeArea.X + tempViewport.TitleSafeArea.Width)) - temp.Left;
            if (Resolution.ViewportY + Resolution.ViewportHeight <= tempViewport.TitleSafeArea.Y + tempViewport.Height) temp.Height = 768 - temp.Top;
            temp.Height = 768 - (DeviceHeight - (tempViewport.TitleSafeArea.Y + tempViewport.TitleSafeArea.Height)) - temp.Top;

            TGPAContext.Instance.TitleSafeArea = temp;
            // like that eh !
#endif
        }

        private static GraphicsDeviceManager _graphics = null;
        private static Matrix _resolutionMatrix = Matrix.Identity;
        private static bool _isFullScreen = false;
        private static bool _dirty = true;
    }
}
