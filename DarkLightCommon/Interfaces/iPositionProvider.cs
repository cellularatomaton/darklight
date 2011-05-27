using System.Collections.Generic;

namespace DarkLight.Common.Interfaces
{
    public delegate void PositionUpdateHandler(int position, int productIndex);

    public interface iPositionProvider
    {
        void RegisterPositionUpdateHandler(PositionUpdateHandler callback);
        void Initialize(List<string> tickers);
        void StartHandlingPositionUpdates();
    }
}
