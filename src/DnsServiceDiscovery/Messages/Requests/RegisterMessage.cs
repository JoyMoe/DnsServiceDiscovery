﻿using System;
using System.IO;

namespace Mittosoft.DnsServiceDiscovery.Messages.Requests
{
    class RegisterMessage : RequestMessage<RegisterMessagePayload>
    {
        public RegisterMessage(ServiceMessageHeader header) : this(header, new RegisterMessagePayload())
        {
        }

        public RegisterMessage(string instanceName, string serviceType, string domain, string host, ushort port, byte[] txtRecord = null, ServiceFlags flags = ServiceFlags.None, uint interfaceIndex = 0)
            : this(new ServiceMessageHeader(OperationCode.RegisterServiceRequest), new RegisterMessagePayload(instanceName, serviceType, domain, host, port,
                txtRecord, flags, interfaceIndex))
        {
        }

        public RegisterMessage(ServiceMessageHeader header, RegisterMessagePayload payload) : base(header, payload)
        {
            if (header.OperationCode != OperationCode.RegisterServiceRequest)
                throw new ArgumentException($"Header contains incorrect operation code [{header.OperationCode}] for this class");
        }
    }

    internal class RegisterMessagePayload : RequestMessagePayload
    {
        public string InstanceName { get; private set; } = string.Empty;
        public string ServiceType { get; private set; } = string.Empty;
        public string Domain { get; private set; } = string.Empty;
        public string HostName { get; private set; } = string.Empty;
        public ushort Port { get; private set; }
        public byte[] TxtRecord { get; private set; }
        public ServiceFlags Flags { get; private set; }
        public uint InterfaceIndex { get; private set; }

        public RegisterMessagePayload()
        {
        }

        public RegisterMessagePayload(string instanceName, string serviceType, string domain, string hostName, ushort port, byte[] txtRecord, ServiceFlags flags, uint interfaceIndex)
        {
            if (string.IsNullOrEmpty(serviceType))
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            InstanceName = instanceName ?? string.Empty;
            ServiceType = serviceType;
            Domain = domain ?? string.Empty;
            HostName = hostName ?? string.Empty;
            Port = port;
            TxtRecord = txtRecord;
            Flags = flags;
            InterfaceIndex = interfaceIndex;
        }

        // From RFC 6763
        // An empty TXT record containing zero strings is not allowed[RFC1035].
        // DNS-SD implementations MUST NOT emit empty TXT records.DNS-SD
        //     clients MUST treat the following as equivalent:
        //
        //  o A TXT record containing a single zero byte.
        //      (i.e., a single empty string.)
        //  o An empty(zero-length) TXT record.
        //      (This is not strictly legal, but should one be received, it should
        //      be interpreted as the same as a single empty string.)
        //  o No TXT record.
        //      (i.e., an NXDOMAIN or no-error-no-answer response.)
        //
        public override byte[] GetBytes()
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);

            writer.Write(base.GetBytes());
            writer.Write(ServiceMessage.HostToNetworkOrder((uint)Flags));
            writer.Write(ServiceMessage.HostToNetworkOrder(InterfaceIndex));
            writer.Write(ServiceMessage.GetMessageStringBytes(InstanceName));
            writer.Write(ServiceMessage.GetMessageStringBytes(ServiceType));
            writer.Write(ServiceMessage.GetMessageStringBytes(Domain));
            writer.Write(ServiceMessage.GetMessageStringBytes(HostName));
            writer.Write(ServiceMessage.HostToNetworkOrder(Port));
            writer.Write(ServiceMessage.HostToNetworkOrder((ushort)(TxtRecord?.Length ?? 0)));
            if (TxtRecord != null)
                writer.Write(TxtRecord);

            return ms.ToArray();
        }

        public override void Parse(byte[] bytes, ref int index)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (index < 0 || index > bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            base.Parse(bytes, ref index);
            Flags = (ServiceFlags)ServiceMessage.GetUInt32(bytes, ref index);
            InterfaceIndex = ServiceMessage.GetUInt32(bytes, ref index);
            InstanceName = ServiceMessage.GetString(bytes, ref index);
            ServiceType = ServiceMessage.GetString(bytes, ref index);
            Domain = ServiceMessage.GetString(bytes, ref index);
            HostName = ServiceMessage.GetString(bytes, ref index);
            Port = ServiceMessage.GetUInt16(bytes, ref index);
            var txtRecordLength = ServiceMessage.GetUInt16(bytes, ref index);
            // Handle zero length record (this isn't strictly legal), or length of 1 which would be a zero length DNS-SD TXT Record
            if (txtRecordLength > 1)
                TxtRecord = ServiceMessage.GetSubArray(bytes, ref index, txtRecordLength);
            else
                index += txtRecordLength;
        }
    }
}
