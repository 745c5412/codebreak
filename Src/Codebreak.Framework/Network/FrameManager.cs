﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Framework.Network
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public sealed class FrameManager<TClient, TMessage> : IDisposable
    {
        private TClient _client;
        private bool _processing;
        private List<IFrame<TClient, TMessage>> _frames, _framesToAdd, _framesToRemove;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _frames.Count == 0 && _framesToAdd.Count == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public FrameManager(TClient client)
        {
            _client = client;
            _processing = false;
            _frames = new List<IFrame<TClient,TMessage>>();
            _framesToAdd = new List<IFrame<TClient,TMessage>>();
            _framesToRemove = new List<IFrame<TClient,TMessage>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool HasFrame(IFrame<TClient, TMessage> frame)
        {
            return _frames.Contains(frame) || _framesToAdd.Contains(frame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ProcessMessage(TMessage message)
        {
            _processing = true;
            var processed = false;

            foreach(var frame in _frames)            
                if(frame.Process(_client, message))
                    processed =  true;

            foreach(var frame in _framesToAdd)
                if(!_frames.Contains(frame))
                    _frames.Add(frame);

            foreach(var frame in _framesToRemove)
                if(_frames.Contains(frame))
                    _frames.Remove(frame);

            _processing = false;

            _framesToAdd.Clear();
            _framesToRemove.Clear();
            
            return processed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void AddFrame(IFrame<TClient, TMessage> frame)
        {
            if (_processing)
                _framesToAdd.Add(frame);
            else
                if(!_frames.Contains(frame))
                    _frames.Add(frame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void RemoveFrame(IFrame<TClient, TMessage> frame)
        {
            if (_processing)
                _framesToRemove.Add(frame);
            else
                if(_frames.Contains(frame))
                    _frames.Remove(frame);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _frames.Clear();
            _frames = null;
            _framesToAdd.Clear();
            _framesToAdd = null;
            _framesToRemove.Clear();
            _framesToRemove = null;
        }
    }
}
