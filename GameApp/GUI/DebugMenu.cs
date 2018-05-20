using System;
using GameCore.GUI;

namespace GameApp.GUI
{
    public class DebugMenu : Control
    {
        private float _frameTime;
        private int _frameCount;
        private float _time;

        [BindControl] public TextControl Fps { get; set; }
        [BindControl] public TextControl PlayerPosition { get; set; }
        [BindControl] public TextControl PlayerChunkPosition { get; set; }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _frameCount++;
            _time += dt;

            if (_time > .2f)
            {
                _frameTime = _time / _frameCount;
                _frameCount = 0;
                _time = 0;
            }

            var player = World.FindByName("Player");

            Fps.Text = $"FPS: {1 / _frameTime:000.0} ({_frameTime * 1000:F5} ms)";
            PlayerPosition.Text = $"Position: {player.Position} View: {player.Rotation / MathF.PI * 180f}";
            PlayerChunkPosition.Text = $"Chunk {player.ChunkPosition} Local pos: {player.ChunkSpacePosition}";
        }
    }
}