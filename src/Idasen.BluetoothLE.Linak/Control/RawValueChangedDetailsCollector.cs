using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Linak.Interfaces;
using JetBrains.Annotations;
using Serilog;

namespace Idasen.BluetoothLE.Linak.Control
{
    public class RawValueChangedDetailsCollector
        : IRawValueChangedDetailsCollector
    {
        private const int MaxItems = 100;

        private readonly IList<HeightSpeedDetails> _details = new List<HeightSpeedDetails>();
        private readonly ILogger                   _logger;
        private readonly IDesk                     _desk;
        private readonly IDisposable               _disposableHeight;

        public RawValueChangedDetailsCollector([NotNull] ILogger          logger,
                                               [NotNull] IScheduler       scheduler,
                                               [NotNull] IDesk desk)
        {
            Guard.ArgumentNotNull(logger,
                                  nameof(logger));
            Guard.ArgumentNotNull(scheduler,
                                  nameof(scheduler));
            Guard.ArgumentNotNull(desk,
                                  nameof(desk));

            _logger = logger;
            _desk       = desk;

            _disposableHeight = _desk.HeightAndSpeedChanged
                                     .SubscribeOn(scheduler)
                                     .Subscribe(OnHeightAndSpeedChanged);
        }

        public IEnumerable<HeightSpeedDetails> Details => _details;

        public void Dispose()
        {
            _disposableHeight?.Dispose();
        }

        private void OnHeightAndSpeedChanged(HeightSpeedDetails details)
        {
            _logger.Debug($"{details}");

            if (_details.Count > MaxItems)
                _details.RemoveAt(0);

            _details.Add(details);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var item in Details)
            {
                builder.AppendLine(item.ToString());
                // todo avoid blank line at the end
            }

            return builder.ToString();
        }
    }
}