using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System.Security.Cryptography;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Utilities
{
    /// <summary>
    /// Utilities related to generating and parsing UUIDs.
    /// </summary>
    public static class UUIDUtilities
    {
        /// <summary>
        /// Takes in a DatabaseObjectType and generates a Version 5 UUID
        /// </summary>
        private static readonly Dictionary<DatabaseObjectTypeEnum, string> NamespacePaths = new()
        {
            [DatabaseObjectTypeEnum.Global_Tenant_Tenant]                                           = "Auxilium.3.DatabaseObject.MariaDb.Global.Tenant.Tenant",



            [DatabaseObjectTypeEnum.WithinTenancy_Calendar_Event]                                   = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Calendar.Event",
            [DatabaseObjectTypeEnum.WithinTenancy_Calendar_EventInvite]                             = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Calendar.EventInvite",



            [DatabaseObjectTypeEnum.WithinTenancy_System_Waf_IpWhitelistEntry]                      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.Waf.IpWhitelistEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_Waf_IpBlacklistEntry]                      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.Waf.IpBlacklistEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_Waf_UserWhitelistEntry]                    = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.Waf.UserWhitelistEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_Waf_UserBlacklistEntry]                    = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.Waf.UserBlacklistEntry",
            
            [DatabaseObjectTypeEnum.WithinTenancy_System_BulletinEntry]                             = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.BulletinEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_MetricEntry]                               = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.MetricEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_SettingEntry]                              = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.SettingEntry",
            


            [DatabaseObjectTypeEnum.WithinTenancy_User]                                             = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User",
            [DatabaseObjectTypeEnum.WithinTenancy_User_AdditionalProperty]                          = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.AdditionalProperty",
            [DatabaseObjectTypeEnum.WithinTenancy_User_File]                                        = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.File",
            [DatabaseObjectTypeEnum.WithinTenancy_User_RefreshToken]                                = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.RefreshToken",
            [DatabaseObjectTypeEnum.WithinTenancy_User_TotpRecoveryCode]                            = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.TotpRecoveryCode",
            [DatabaseObjectTypeEnum.WithinTenancy_User_WemwbsAssessment]                            = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.WemwbsAssessment",
            [DatabaseObjectTypeEnum.WithinTenancy_User_PasswordSetToken]                            = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.PasswordSetToken",
            


            [DatabaseObjectTypeEnum.WithinTenancy_Case]                                             = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_AdditionalProperty]                          = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.AdditionalProperty",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_Worker]                                      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.Worker",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_Client]                                      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.Client",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_Message]                                     = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.Message",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_File]                                        = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.File",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_TimelineEntry]                               = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.TimelineEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_Todo]                                        = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.Todo",



            [DatabaseObjectTypeEnum.WithinTenancy_Enumerator]                                       = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Enumerator",
            [DatabaseObjectTypeEnum.WithinTenancy_Enumerator_Translation]                           = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Enumerator.Translation",
            [DatabaseObjectTypeEnum.WithinTenancy_Enumerator_Value]                                 = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Enumerator.Value",
            [DatabaseObjectTypeEnum.WithinTenancy_Enumerator_ValueTranslation]                      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Enumerator.ValueTranslation",



            [DatabaseObjectTypeEnum.WithinTenancy_Log_CaseModification_EventEntry]                  = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.CaseModification.EventEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMessageReadBy_EventEntry]                 = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.CaseMessageReadBy.EventEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMerge_EventEntry]                         = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.CaseMerge.EventEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMerge_RowChange_EventEntry]               = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.CaseMerge.RowChange.EventEntry",

            [DatabaseObjectTypeEnum.WithinTenancy_Log_LoginAttempt_EventEntry]                      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.LoginAttempt.EventEntry",

            [DatabaseObjectTypeEnum.WithinTenancy_Log_SystemMessageQueue_EmailSent_EventEntry]      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.System.MessageQueue.EmailSent.EventEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_SystemMessageQueue_FailedAction_EventEntry]   = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.System.MessageQueue.FailedAction.EventEntry",

            [DatabaseObjectTypeEnum.WithinTenancy_Log_SystemBulletin_EntryDismissal_EventEntry]     = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.System.BulletinEntry.Dismissal.EventEntry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_SystemBulletin_EntryView_EventEntry]          = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.System.BulletinEntry.View.EventEntry",

            [DatabaseObjectTypeEnum.WithinTenancy_Log_UserModification_EventEntry]                  = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.UserModification.EventEntry",
        };


        /// <summary>
        /// Takes in a DatabaseObjectType and gets the precomputed Namespace UUID for it.
        /// </summary>
        private static readonly Dictionary<DatabaseObjectTypeEnum, Guid> NamespaceUuids =
            NamespacePaths.ToDictionary(
                kvp => kvp.Key,
                kvp => PathToUuid(kvp.Value)
            );

        /// <summary>
        /// Generates a Version 5 UUID for the given DatabaseObjectType.
        /// </summary>
        /// <param name="objectType">The DatabaseObjectType to use</param>
        /// <returns>A UUID (Guid).</returns>
        public static Guid GenerateV5(DatabaseObjectTypeEnum objectType)
        {
            var namespaceId = NamespaceUuids[objectType];
            var name = $"{objectType}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";

            return GenerateV5(namespaceId, name);
        }

        /// <summary>
        /// Generates a Version 5 UUID given a namespace UUID and a name.
        /// </summary>
        /// <param name="namespaceId">The UUID (Guid) that represents the namespace to use.</param>
        /// <param name="name">The name to use.</param>
        /// <returns>A UUID (Guid).</returns>
        public static Guid GenerateV5(Guid namespaceId, string name)
        {
            var namespaceBytes = namespaceId.ToByteArray();
            var nameBytes = Encoding.UTF8.GetBytes(name);

            SwapByteOrder(namespaceBytes);

            var hash = SHA1.HashData(namespaceBytes.Concat(nameBytes).ToArray());

            var newGuid = new byte[16];
            Array.Copy(hash, 0, newGuid, 0, 16);

            newGuid[6] = (byte)(newGuid[6] & 0x0F | 0x50);
            newGuid[8] = (byte)(newGuid[8] & 0x3F | 0x80);

            SwapByteOrder(newGuid);

            return new Guid(newGuid);
        }

        /// <summary>
        /// Converts a path string to a Version 5 UUID using SHA-1 hashing.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Guid PathToUuid(string path)
        {
            var hash = SHA1.HashData(Encoding.UTF8.GetBytes(path));

            var uuid = new byte[16];
            Array.Copy(hash, 0, uuid, 0, 16);

            uuid[6] = (byte)(uuid[6] & 0x0F | 0x50);
            uuid[8] = (byte)(uuid[8] & 0x3F | 0x80);

            return new Guid(uuid);
        }

        /// <summary>
        /// Takes in a GUID byte array and swaps the byte order to match RFC 4122.
        /// </summary>
        /// <param name="guid">A 16-byte array representing a GUID. The array is modified in place.</param>
        private static void SwapByteOrder(byte[] guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);
        }

        /// <summary>
        /// Swaps two bytes in a byte array.
        /// </summary>
        /// <param name="guid">The byte array to target for swapping bytes.</param>
        /// <param name="left">The index of the first byte to swap.</param>
        /// <param name="right">The index of the second byte to swap.</param>
        private static void SwapBytes(byte[] guid, int left, int right)
        {
            (guid[left], guid[right]) = (guid[right], guid[left]);
        }
    }
}
