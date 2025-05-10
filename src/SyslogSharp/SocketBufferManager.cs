using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SyslogSharp;
internal sealed class SocketBufferManager
{
    private readonly int _numBytes;
    private readonly byte[] _buffer;
    private readonly Stack<int> _freeIndexPool;
    private int _currentIndex;
    private readonly int _bufferSize;

    public SocketBufferManager(int totalBytes, int bufferSize)
    {
        _numBytes = totalBytes;
        _currentIndex = 0;
        _bufferSize = bufferSize;
        _freeIndexPool = new Stack<int>();
        _buffer = new byte[_numBytes];
    }

    public bool SetBuffer(SocketAsyncEventArgs args)
    {
        if(_freeIndexPool.Count > 0)
        {
            args.SetBuffer(_buffer, _freeIndexPool.Pop(), _bufferSize);
        }
        else
        {
            if((_numBytes - _bufferSize) < _currentIndex)
            {
                return false;
            }

            args.SetBuffer(_buffer, _currentIndex, _bufferSize);
            _currentIndex += _bufferSize;
        }

        return true;
    }

    public void FreeBuffer(SocketAsyncEventArgs args)
    {
        _freeIndexPool.Push(args.Offset);
        args.SetBuffer(null, 0, 0);
    }
}
