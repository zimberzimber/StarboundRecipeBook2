using Jil;
using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB.Seeder.Extensions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace SBRB.Seeder
{
    partial class Program
    {
        const string CURRENCY_FILE = "currencies.config";

        static ConcurrentQueue<Currency> _DBCurrencies = new ConcurrentQueue<Currency>();

        /// <summary>
        /// Deserialize and add base game currencies into the database.
        /// </summary>
        static void DeserializeCurrency()
        {
            // Not applying much logic and safeguards here because only the base game assets will use this method.
            // And that doesn't change at all. (Unless they release an update, in which case I'll have to revise the entire program)
            // Lazy, but really no point in putting effort here.

            if (!File.Exists($"{modPath}\\{CURRENCY_FILE}"))
            {
                _logger.Log("No currency file detected.");
                return;
            }

            string json = File.ReadAllText($"{modPath}\\{CURRENCY_FILE}");
            var currencies = JSON.Deserialize<DeserializedBaseGameCurrencyFile>(json);

            _DBCurrencies.Enqueue(new Currency
            {
                ID = new CompositeCurrencyId { SourceModId = _mod.SteamId, CurrencyName = "money" },
                PlayerMax = currencies.money.playerMax,
                RepresentativeItem = currencies.money.representativeItem,
            });

            _DBCurrencies.Enqueue(new Currency
            {
                ID = new CompositeCurrencyId { SourceModId = _mod.SteamId, CurrencyName = "essence" },
                PlayerMax = currencies.essence.playerMax,
                RepresentativeItem = currencies.essence.representativeItem,
            });
        }

        /// <summary>
        /// Deserialize and add patched currencies into the database
        /// </summary>
        static void DeserializeCurrencyPatch()
        {
            // Stop if there is no currency patch file
            if (!File.Exists($"{modPath}\\{CURRENCY_FILE}.patch"))
            {
                _logger.Log("No currency patch file detected.");
                return;
            }

            // Encapsulate the patch contents in an object because a patches root is an array instead of an object
            string patchJson = File.ReadAllText($"{modPath}\\{CURRENCY_FILE}.patch").RemoveComments().LegitizimeJsonPatch();
            DeserializedPatchFile patchFile = JSON.Deserialize<DeserializedPatchFile>(patchJson);

            foreach (var item in patchFile.contents)
            {
                // Make sure its an add operation targeting root. Otherwise its not an added currency.
                if (item.op.Equals("add", StringComparison.OrdinalIgnoreCase) && item.path.Count(c => c == '/') == 1)
                {
                    DeserializedCurrency currency = null;
                    var currencyName = item.path.Replace("/", "");
                    var representingItem = item.value.representativeItem;

                    try
                    {
                        // Attempt converting the to-be-patched value into the deserialized currency class
                        // This may fail during runtime, depending on the contents.
                        string json = item.value.ToString();
                        currency = JSON.Deserialize<DeserializedCurrency>(json);

                        // Add the currency into the queue
                        _DBCurrencies.Enqueue(new Currency
                        {
                            ID = new CompositeCurrencyId { SourceModId = _mod.SteamId, CurrencyName = currencyName },
                            PlayerMax = currency.playerMax,
                            RepresentativeItem = currency.representativeItem,
                        });
                    }
                    catch (Exception e)
                    {
                        _logger.Log($"An exception occured while deserializing the patch contents targeting '{item.path}':");
                        _logger.Log(e.Message);
                    }
                }
                else
                    _logger.Log($"Could not proccess '{item.path}' as currency.");
            }
        }
    }
}
