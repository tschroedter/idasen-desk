namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskHeightMonitor
    {
        bool IsHeightChanging() ;
        void Reset ( ) ;
        void AddHeight ( uint height ) ;
    }
}