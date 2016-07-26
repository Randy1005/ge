﻿using System;
using System.Numerics;
using Veldrid.Graphics;
using Veldrid.Graphics.Pipeline;

namespace Ge.Graphics
{
    public class ShadowMapStage : PipelineStage
    {
        private string _contextBindingName = "ShadowMap";

        private const int DepthMapWidth = 4096;
        private const int DepthMapHeight = 4096;

        private readonly RenderQueue _queue = new RenderQueue();

        private Framebuffer _shadowMapFramebuffer;
        private DeviceTexture2D _depthTexture;

        public bool Enabled { get; set; } = true;

        public string Name => "ShadowMap";

        public RenderContext RenderContext { get; private set; }

        public ShadowMapStage(RenderContext rc, string contextBindingName = "ShadowMap")
        {
            RenderContext = rc;
            _contextBindingName = contextBindingName;
            InitializeContextObjects(rc);
        }

        public void ChangeRenderContext(RenderContext rc)
        {
            RenderContext = rc;
            InitializeContextObjects(rc);
        }

        private void InitializeContextObjects(RenderContext rc)
        {
            _depthTexture = rc.ResourceFactory.CreateDepthTexture(DepthMapWidth, DepthMapHeight, sizeof(ushort), PixelFormat.Alpha_UInt16);
            _shadowMapFramebuffer = rc.ResourceFactory.CreateFramebuffer();
            _shadowMapFramebuffer.DepthTexture = _depthTexture;
            rc.GetTextureContextBinding(_contextBindingName).Value = _depthTexture;
        }

        public void ExecuteStage(VisibiltyManager visibilityManager, Vector3 cameraPosition)
        {
            RenderContext.ClearScissorRectangle();
            RenderContext.SetFramebuffer(_shadowMapFramebuffer);
            RenderContext.ClearBuffer();
            RenderContext.SetViewport(0, 0, DepthMapWidth, DepthMapHeight);
            _queue.Clear();
            visibilityManager.CollectVisibleObjects(_queue, "ShadowMap", Vector3.Zero);
            _queue.Sort();

            foreach (RenderItem item in _queue)
            {
                item.Render(RenderContext, "ShadowMap");
            }
        }

        private void Dispose()
        {
            _shadowMapFramebuffer.Dispose();
        }
    }
}