﻿/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.UI;
using MatterHackers.PolygonMesh;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
    public class PartPreviewWidget : GuiWidget
    {
        protected readonly int ShortButtonHeight = 25;
        protected readonly int SideBarButtonWidth = 138;

        protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
        protected TextImageButtonFactory checkboxButtonFactory = new TextImageButtonFactory();
        protected TextImageButtonFactory expandMenuOptionFactory = new TextImageButtonFactory();
        protected TextImageButtonFactory whiteButtonFactory = new TextImageButtonFactory();

        protected ViewControls2D viewControls2D;

        protected Cover buttonRightPanelDisabledCover;
        protected FlowLayoutWidget buttonRightPanel;

        public PartPreviewWidget()
        {
            textImageButtonFactory.normalTextColor = ActiveTheme.Instance.PrimaryTextColor;
            textImageButtonFactory.hoverTextColor = ActiveTheme.Instance.PrimaryTextColor;
            textImageButtonFactory.disabledTextColor = ActiveTheme.Instance.PrimaryTextColor;
            textImageButtonFactory.pressedTextColor = ActiveTheme.Instance.PrimaryTextColor;

            whiteButtonFactory.FixedWidth = SideBarButtonWidth;
            whiteButtonFactory.FixedHeight = ShortButtonHeight;
            whiteButtonFactory.normalFillColor = RGBA_Bytes.White;
            whiteButtonFactory.normalTextColor = RGBA_Bytes.Black;
            whiteButtonFactory.hoverTextColor = RGBA_Bytes.Black;
            whiteButtonFactory.hoverFillColor = new RGBA_Bytes(255, 255, 255, 200);

            expandMenuOptionFactory.FixedWidth = SideBarButtonWidth;
            expandMenuOptionFactory.normalTextColor = ActiveTheme.Instance.PrimaryTextColor;
            expandMenuOptionFactory.hoverTextColor = ActiveTheme.Instance.PrimaryTextColor;
            expandMenuOptionFactory.disabledTextColor = ActiveTheme.Instance.PrimaryTextColor;
            expandMenuOptionFactory.pressedTextColor = ActiveTheme.Instance.PrimaryTextColor;
            expandMenuOptionFactory.hoverFillColor = new RGBA_Bytes(255, 255, 255, 50);
            expandMenuOptionFactory.pressedFillColor = new RGBA_Bytes(255, 255, 255, 50);
            expandMenuOptionFactory.disabledFillColor = new RGBA_Bytes(255, 255, 255, 50);

            checkboxButtonFactory.fontSize = 11;
            checkboxButtonFactory.FixedWidth = SideBarButtonWidth;
            checkboxButtonFactory.borderWidth = 3;

            checkboxButtonFactory.normalTextColor = ActiveTheme.Instance.PrimaryTextColor;
            checkboxButtonFactory.normalBorderColor = new RGBA_Bytes(0, 0, 0, 0);
            checkboxButtonFactory.normalFillColor = ActiveTheme.Instance.PrimaryBackgroundColor;

            checkboxButtonFactory.hoverTextColor = ActiveTheme.Instance.PrimaryTextColor;
            checkboxButtonFactory.hoverBorderColor = new RGBA_Bytes(0, 0, 0, 50);
            checkboxButtonFactory.hoverFillColor = new RGBA_Bytes(0, 0, 0, 50);

            checkboxButtonFactory.pressedTextColor = ActiveTheme.Instance.PrimaryTextColor;
            checkboxButtonFactory.pressedBorderColor = new RGBA_Bytes(0, 0, 0, 50);

            checkboxButtonFactory.disabledTextColor = ActiveTheme.Instance.PrimaryTextColor;

            BackgroundColor = RGBA_Bytes.White;
        }
    }
}
