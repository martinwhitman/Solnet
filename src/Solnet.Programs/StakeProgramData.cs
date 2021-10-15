﻿using Solnet.Programs.Models;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Solnet.Programs.Models.Stake.Instruction;
using static Solnet.Programs.Models.Stake.State;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the stake program data encodings.
    /// </summary>
    internal static class StakeProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the program method begins. 
        /// </summary>
        internal const int MethodOffset = 0;
        /// <summary>
        /// Summary text here
        /// </summary>
        internal static byte[] EncodeInitializeData(Authorized authorized, Lockup lockup)
        {
            byte[] data = new byte[116];

            data.WriteU32((uint)StakeProgramInstructions.Values.Initialize, MethodOffset);
            data.WritePubKey(authorized.staker,4);
            data.WritePubKey(authorized.withdrawer,36);
            data.WriteS64(lockup.unix_timestamp, 68);
            data.WriteU64(lockup.epoch, 76);
            data.WritePubKey(lockup.custodian, 84);

            return data;
        }
        internal static byte[] EncodeAuthorizeData(PublicKey new_authorized_pubkey, StakeAuthorize stake_authorize)
        {
            byte[] data = new byte[68];

            data.WriteU32((uint)StakeProgramInstructions.Values.Authorize, MethodOffset);
            data.WritePubKey(new_authorized_pubkey, 4);
            data.WriteU32((uint)stake_authorize, 36);

            return data;
        }

        internal static byte[] EncodeDelegateStakeData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.DelegateStake, MethodOffset);

            return data;
        }
        internal static byte[] EncodeSplitData(ulong lamports)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)StakeProgramInstructions.Values.Split, MethodOffset);
            data.WriteU64(lamports, 4);

            return data;
        }

        internal static byte[] EncodeWithdrawData(ulong lamports)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)StakeProgramInstructions.Values.Withdraw, MethodOffset);
            data.WriteU64(lamports, 4);

            return data;
        }

        internal static byte[] EncodeDeactivateData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.Deactivate, MethodOffset);

            return data;
        }

        internal static byte[] EncodeSetLockupData(Lockup lockup)
        {
            byte[] data = new byte[52];

            data.WriteU32((uint)StakeProgramInstructions.Values.SetLockup, MethodOffset);
            data.WriteS64(lockup.unix_timestamp, 4);
            data.WriteU64(lockup.epoch, 12);
            data.WritePubKey(lockup.custodian, 20);

            return data;
        }

        internal static byte[] EncodeMergeData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.Merge, MethodOffset);

            return data;
        }

        internal static byte[] EncodeAuthorizeWithSeedData(AuthorizeWithSeedArgs authorizeWithSeed)
        {
            byte[] encodedSeed = Serialization.EncodeRustString(authorizeWithSeed.authority_seed);
            byte[] data = new byte[72 + encodedSeed.Length];

            data.WriteU32((uint)StakeProgramInstructions.Values.AuthorizeWithSeed, MethodOffset);
            data.WritePubKey(authorizeWithSeed.new_authorized_pubkey,4);
            data.WriteSpan(encodedSeed,36);
            data.WriteU32((uint)authorizeWithSeed.stake_authorize,36+encodedSeed.Length);
            data.WritePubKey(authorizeWithSeed.authority_owner,40+encodedSeed.Length);

            return data;
        }

        internal static byte[] EncodeInitializeCheckedData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.InitializeChecked, MethodOffset);

            return data;
        }

        internal static byte[] EncodeAuthorizeCheckedData(StakeAuthorize stake_authorize)
        {
            byte[] data = new byte[8];

            data.WriteU32((uint)StakeProgramInstructions.Values.Authorize, MethodOffset);
            data.WriteU32((uint)stake_authorize, 4);

            return data;
        }

        internal static byte[] EncodeAuthorizeCheckedWithSeedData(AuthorizeCheckedWithSeedArgs authorizeCheckedWithSeed)
        {
            byte[] encodedSeed = Serialization.EncodeRustString(authorizeCheckedWithSeed.authority_seed);
            byte[] data = new byte[40 + encodedSeed.Length];

            data.WriteU32((uint)StakeProgramInstructions.Values.AuthorizeCheckedWithSeed, MethodOffset);
            data.WriteSpan(encodedSeed, 4);
            data.WriteU32((uint)authorizeCheckedWithSeed.stake_authorize, 4 + encodedSeed.Length);
            data.WritePubKey(authorizeCheckedWithSeed.authority_owner, 8 + encodedSeed.Length);

            return data;
        }
        internal static byte[] EncodeSetLockupCheckedData(LockupChecked lockup)
        {
            byte[] data = new byte[20];

            data.WriteU32((uint)StakeProgramInstructions.Values.SetLockup, MethodOffset);
            data.WriteS64(lockup.unix_timestamp, 4);
            data.WriteU64(lockup.epoch, 12);

            return data;
        }
        internal static void DecodeInitializeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authorized Withdraw Account", data.GetPubKey(4));
            decodedInstruction.Values.Add("Authorized Stake Account", data.GetPubKey(36));
            decodedInstruction.Values.Add("Lockup Timestamp", data.GetS64(68));
            decodedInstruction.Values.Add("Lockup Epoch", data.GetU64(76));
            decodedInstruction.Values.Add("Lockup Custodian", data.GetPubKey(84));
        }
        internal static void DecodeAuthorizeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("New Authorized Account", data.GetPubKey(4));
            decodedInstruction.Values.Add("Stake Authorize", Enum.Parse(typeof(StakeAuthorize), data.GetU8(36).ToString()));
        }
        internal static void DecodeDelegateStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Vote Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[5]]);
        }
        internal static void DecodeSplitStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Split Stake Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(4));
        }
        internal static void DecodeWithdrawStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("To Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Withdraw Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(4));
        }
        internal static void DecodeDeactivateStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[2]]);
        }
    }
}
