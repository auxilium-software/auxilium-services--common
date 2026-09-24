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
            [DatabaseObjectTypeEnum.Global_Tenant_Tenant]                                           = "/auxilium/3/database-object/mariadb/global/tenant/tenant",



            [DatabaseObjectTypeEnum.WithinTenancy_Calendar_Event]                                   = "/auxilium/3/database-object/mariadb/within-tenancy/calendar/event",
            [DatabaseObjectTypeEnum.WithinTenancy_Calendar_EventInvite]                             = "/auxilium/3/database-object/mariadb/within-tenancy/calendar/event-invite",



            [DatabaseObjectTypeEnum.WithinTenancy_System_Waf_IpWhitelistEntry]                      = "/auxilium/3/database-object/mariadb/within-tenancy/system/waf/ip-whitelist-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_Waf_IpBlacklistEntry]                      = "/auxilium/3/database-object/mariadb/within-tenancy/system/waf/ip-blacklist-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_Waf_UserWhitelistEntry]                    = "/auxilium/3/database-object/mariadb/within-tenancy/system/waf/user-whitelist-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_Waf_UserBlacklistEntry]                    = "/auxilium/3/database-object/mariadb/within-tenancy/system/waf/user-blacklist-entry",
            
            [DatabaseObjectTypeEnum.WithinTenancy_System_BulletinEntry]                             = "/auxilium/3/database-object/mariadb/within-tenancy/system/bulletin-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_MetricEntry]                               = "/auxilium/3/database-object/mariadb/within-tenancy/system/metric-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_System_SettingEntry]                              = "/auxilium/3/database-object/mariadb/within-tenancy/system/setting-entry",
            


            [DatabaseObjectTypeEnum.WithinTenancy_User]                                             = "/auxilium/3/database-object/mariadb/within-tenancy/user",
            [DatabaseObjectTypeEnum.WithinTenancy_User_AdditionalProperty]                          = "/auxilium/3/database-object/mariadb/within-tenancy/user/additional-property",
            [DatabaseObjectTypeEnum.WithinTenancy_User_File]                                        = "/auxilium/3/database-object/mariadb/within-tenancy/user/file",
            [DatabaseObjectTypeEnum.WithinTenancy_User_RefreshToken]                                = "/auxilium/3/database-object/mariadb/within-tenancy/user/refresh-token",
            [DatabaseObjectTypeEnum.WithinTenancy_User_TotpRecoveryCode]                            = "/auxilium/3/database-object/mariadb/within-tenancy/user/totp-recovery-code",
            [DatabaseObjectTypeEnum.WithinTenancy_User_WemwbsAssessment]                            = "/auxilium/3/database-object/mariadb/within-tenancy/user/wemwbs-assessment",
            [DatabaseObjectTypeEnum.WithinTenancy_User_PasswordSetToken]                            = "/auxilium/3/database-object/mariadb/within-tenancy/user/password-set-token",
            


            [DatabaseObjectTypeEnum.WithinTenancy_Case]                                             = "/auxilium/3/database-object/mariadb/within-tenancy/case",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_AdditionalProperty]                          = "/auxilium/3/database-object/mariadb/within-tenancy/case/additional-property",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_Worker]                                      = "/auxilium/3/database-object/mariadb/within-tenancy/case/worker",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_Client]                                      = "/auxilium/3/database-object/mariadb/within-tenancy/case/client",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_Message]                                     = "/auxilium/3/database-object/mariadb/within-tenancy/case/message",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_File]                                        = "/auxilium/3/database-object/mariadb/within-tenancy/case/file",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_TimelineEntry]                               = "/auxilium/3/database-object/mariadb/within-tenancy/case/timeline-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_Case_Todo]                                        = "/auxilium/3/database-object/mariadb/within-tenancy/case/todo",



            [DatabaseObjectTypeEnum.WithinTenancy_Enumerator]                                       = "/auxilium/3/database-object/mariadb/within-tenancy/enumerator",
            [DatabaseObjectTypeEnum.WithinTenancy_Enumerator_Translation]                           = "/auxilium/3/database-object/mariadb/within-tenancy/enumerator/translation",
            [DatabaseObjectTypeEnum.WithinTenancy_Enumerator_Value]                                 = "/auxilium/3/database-object/mariadb/within-tenancy/enumerator/value",
            [DatabaseObjectTypeEnum.WithinTenancy_Enumerator_ValueTranslation]                      = "/auxilium/3/database-object/mariadb/within-tenancy/enumerator/value-translation",



            [DatabaseObjectTypeEnum.WithinTenancy_Log_CaseModification_EventEntry]                  = "/auxilium/3/database-object/mariadb/within-tenancy/log/case-modification/event-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMessageReadBy_EventEntry]                 = "/auxilium/3/database-object/mariadb/within-tenancy/log/case-message-read-by/event-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMerge_EventEntry]                         = "/auxilium/3/database-object/mariadb/within-tenancy/log/case-merge/event-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMerge_RowChange]                          = "/auxilium/3/database-object/mariadb/within-tenancy/log/case-merge/row-change",

            [DatabaseObjectTypeEnum.WithinTenancy_Log_LoginAttempt_EventEntry]                      = "/auxilium/3/database-object/mariadb/within-tenancy/log/login-attempt/event-entry",

            [DatabaseObjectTypeEnum.WithinTenancy_Log_SystemMessageQueue_EmailSent_EventEntry]      = "/auxilium/3/database-object/mariadb/within-tenancy/log/system/message-queue/email-sent/event-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_SystemMessageQueue_FailedAction_EventEntry]   = "/auxilium/3/database-object/mariadb/within-tenancy/log/system/message-queue/failed-action/event-entry",

            [DatabaseObjectTypeEnum.WithinTenancy_Log_SystemBulletin_EntryDismissal_EventEntry]     = "/auxilium/3/database-object/mariadb/within-tenancy/log/system/bulletin-entry/dismissal/event-entry",
            [DatabaseObjectTypeEnum.WithinTenancy_Log_SystemBulletin_EntryView_EventEntry]          = "/auxilium/3/database-object/mariadb/within-tenancy/log/system/bulletin-entry/view/event-entry",

            [DatabaseObjectTypeEnum.WithinTenancy_Log_UserModification_EventEntry]                  = "/auxilium/3/database-object/mariadb/within-tenancy/log/user-modification/event-entry",
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
