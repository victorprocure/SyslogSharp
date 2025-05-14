namespace SyslogSharp.Networking;

/// <summary>
/// Represents the protocol types defined by the Internet Assigned Numbers Authority (IANA) for use in IP networking.
/// </summary>
/// <remarks>This enumeration provides a comprehensive list of protocol numbers as defined in the IANA Protocol
/// Numbers registry. Each value corresponds to a specific protocol used in IP networking, such as TCP, UDP, ICMP, and
/// others. These protocol numbers are used in the Protocol field of the IP header to identify the next level
/// protocol.</remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "Analyzer bugging out")]
internal enum ProtocolType : byte
{
    /// <summary>
    /// Represents the IPv6 Hop-by-Hop Option (HOPOPT) protocol, identified by the protocol number 0.
    /// </summary>
    /// <remarks>The HOPOPT protocol is used in IPv6 to carry optional information that must be examined by
    /// every node along a packet's delivery path.</remarks>
    HOPOPT = 0,           // IPv6 Hop-by-Hop Option

    /// <summary>
    /// Represents the Internet Control Message Protocol (ICMP) protocol type.
    /// </summary>
    /// <remarks>ICMP is a network layer protocol used for error messages and operational information queries,
    /// such as determining whether a host is reachable.</remarks>
    ICMP = 1,             // Internet Control Message Protocol

    /// <summary>
    /// Represents the Internet Group Management Protocol (IGMP) with a protocol number of 2.
    /// </summary>
    /// <remarks>IGMP is used by hosts and adjacent routers on IPv4 networks to establish multicast group
    /// memberships.</remarks>
    IGMP = 2,             // Internet Group Management Protocol

    /// <summary>
    /// Represents the GGP (Greatest Good Point) enumeration value.
    /// </summary>
    GGP = 3,              // Gateway-to-Gateway Protocol

    /// <summary>
    /// Represents the Internet Protocol version 4 (IPv4).
    /// </summary>
    IPv4 = 4,             // IPv4 encapsulation

    /// <summary>
    /// Represents the Stream Protocol (ST) with a protocol number of 5.
    /// </summary>
    ST = 5,               // Stream

    /// <summary>
    /// Represents the Transmission Control Protocol (TCP) with a protocol number of 6.
    /// </summary>
    TCP = 6,              // Transmission Control Protocol

    /// <summary>
    /// Represents the Protocol Type Control Block Transfer (CBT) with a protocol number of 7.
    /// </summary>
    CBT = 7,              // CBT

    /// <summary>
    /// Represents the protocol number for Exterior Gateway Protocol (EGP).
    /// </summary>
    EGP = 8,              // Exterior Gateway Protocol

    /// <summary>
    /// Represents the Interior Gateway Protocol (IGP) routing type.
    /// </summary>
    /// <remarks>IGP is a type of routing protocol used within an autonomous system to exchange routing
    /// information.</remarks>
    IGP = 9,              // Interior Gateway Protocol

    /// <summary>
    /// Represents the protocol type for Bolt, Beranek, and Newman Inc. (BBN)  Reporting and Control Center (RCC) Monitoring
    /// </summary>
    BBN_RCC_MON = 10,     // BBN RCC Monitoring

    /// <summary>
    /// Represents the network voice protocol (NVP) with a protocol number of 11.
    /// </summary>
    NVP_II = 11,          // Network Voice Protocol

    /// <summary>
    /// Represents the PARC Universal Packet (PUP) protocol with a protocol number of 12.
    /// </summary>
    PUP = 12,             // PUP

    /// <summary>
    /// Represents the ARGUS constant with a value of 13.
    /// </summary>
    ARGUS = 13,           // ARGUS

    /// <summary>
    /// Represents the EMCON (Emission Control) state with a value of 14.
    /// </summary>
    /// <remarks>EMCON is typically used in scenarios where emission control is required, such as in military
    /// or sensitive communication environments.</remarks>
    EMCON = 14,           // EMCON

    /// <summary>
    /// Represents the cross net debugger (XNET) protocol with a value of 15.
    /// </summary>
    XNET = 15,            // Cross Net Debugger

    /// <summary>
    /// Represents the chaos protocol with a value of 16.
    /// </summary>
    CHAOS = 16,           // Chaos

    /// <summary>
    /// Represents the User Datagram Protocol (UDP) with a protocol number of 17.
    /// </summary>
    /// <remarks>UDP is a connectionless transport layer protocol used for low-latency and loss-tolerating connections.</remarks>
    UDP = 17,             // User Datagram Protocol

    /// <summary>
    /// Represents the Multiplexing (MUX) protocol with a protocol number of 18.
    /// </summary>
    MUX = 18,             // Multiplexing

    /// <summary>
    /// Represents the DCN Measurement Subsystems protocol with a protocol number of 19.
    /// </summary>
    DCN_MEAS = 19,        // DCN Measurement Subsystems

    /// <summary>
    /// Represents the Host Monitoring Protocol (HMP) with a protocol number of 20.
    /// </summary>
    HMP = 20,             // Host Monitoring Protocol

    /// <summary>
    /// Represents the Packet Radio Measurement (PRM) protocol with a protocol number of 21.
    /// </summary>
    PRM = 21,             // Packet Radio Measurement

    /// <summary>
    /// Represents the XEROX NS IDP protocol with a protocol number of 22.
    /// </summary>
    XNS_IDP = 22,         // XEROX NS IDP

    /// <summary>
    /// Represents the Trunk-1 protocol with a protocol number of 23.
    /// </summary>
    TRUNK_1 = 23,         // Trunk-1

    /// <summary>
    /// Represents the Trunk-2 protocol with a protocol number of 24.
    /// </summary>
    TRUNK_2 = 24,         // Trunk-2

    /// <summary>
    /// Represents the Leaf-1 protocol with a protocol number of 25.
    /// </summary>
    LEAF_1 = 25,          // Leaf-1

    /// <summary>
    /// Represents the Leaf-2 protocol with a protocol number of 26.
    /// </summary>
    LEAF_2 = 26,          // Leaf-2

    /// <summary>
    /// Represents the Reliable Data Protocol (RDP) with a protocol number of 27.
    /// </summary>
    RDP = 27,             // Reliable Data Protocol

    /// <summary>
    /// Represents the Internet Reliable Transaction Protocol (IRTP) with a protocol number of 28.
    /// </summary>
    IRTP = 28,            // Internet Reliable Transaction

    /// <summary>
    /// Represents the ISO Transport Protocol Class 4 (ISO-TP4) with a protocol number of 29.
    /// </summary>
    ISO_TP4 = 29,         // ISO Transport Protocol Class 4

    /// <summary>
    /// Represents the Bulk Data Transfer Protocol (NETBLT) with a protocol number of 30.
    /// </summary>
    NETBLT = 30,          // Bulk Data Transfer Protocol

    /// <summary>
    /// Represents the MFE Network Services Protocol (MFE-NSP) with a protocol number of 31.
    /// </summary>
    MFE_NSP = 31,         // MFE Network Services Protocol

    /// <summary>
    /// Represents the MERIT Internodal Protocol (MERIT-INP) with a protocol number of 32.
    /// </summary>
    MERIT_INP = 32,       // MERIT Internodal Protocol

    /// <summary>
    /// Represents the Datagram Congestion Control Protocol (DCCP) with a protocol number of 33.
    /// </summary>
    DCCP = 33,            // Datagram Congestion Control Protocol

    /// <summary>
    /// Represents the Third Party Connect Protocol (3PC) with a protocol number of 34.
    /// </summary>
    THREE_PC = 34,        // Third Party Connect Protocol

    /// <summary>
    /// Represents the Inter-Domain Policy Routing Protocol (IDPR) with a protocol number of 35.
    /// </summary>
    IDPR = 35,            // Inter-Domain Policy Routing Protocol

    /// <summary>
    /// Represents the Xpress Transfer Protocol (XTP) with a protocol number of 36.
    /// </summary>
    XTP = 36,             // XTP

    /// <summary>
    /// Represents the Datagram Delivery Protocol (DDP) with a protocol number of 37.
    /// </summary>
    DDP = 37,             // Datagram Delivery Protocol

    /// <summary>
    /// Represents the IDPR Control Message Transport Protocol (IDPR-CMTP) with a protocol number of 38.
    /// </summary>
    IDPR_CMTP = 38,       // IDPR Control Message Transport Proto

    /// <summary>
    /// Represents the TP++ Transport Protocol (TP_PLUS_PLUS) with a protocol number of 39.
    /// </summary>
    TP_PLUS_PLUS = 39,    // TP++ Transport Protocol

    /// <summary>
    /// Represents the IL Transport Protocol (IL) with a protocol number of 40.
    /// </summary>
    IL = 40,              // IL Transport Protocol

    /// <summary>
    /// Represents the Internet Protocol version 6 (IPv6) encapsulation with a protocol number of 41.
    /// </summary>
    IPv6 = 41,            // IPv6 encapsulation

    /// <summary>
    /// Represents the Source Demand Routing Protocol (SDRP) with a protocol number of 42.
    /// </summary>
    SDRP = 42,            // Source Demand Routing Protocol

    /// <summary>
    /// Represents the Routing Header for IPv6 (IPv6-Route) with a protocol number of 43.
    /// </summary>
    IPv6_Route = 43,      // Routing Header for IPv6

    /// <summary>
    /// Represents the Fragment Header for IPv6 (IPv6-Frag) with a protocol number of 44.
    /// </summary>
    IPv6_Frag = 44,       // Fragment Header for IPv6

    /// <summary>
    /// Represents the Inter-Domain Routing Protocol (IDRP) with a protocol number of 45.
    /// </summary>
    IDRP = 45,            // Inter-Domain Routing Protocol

    /// <summary>
    /// Represents the Reservation Protocol (RSVP) with a protocol number of 46.
    /// </summary>
    RSVP = 46,            // Reservation Protocol

    /// <summary>
    /// Represents the Generic Routing Encapsulation (GRE) protocol with a protocol number of 47.
    /// </summary>
    GRE = 47,             // Generic Routing Encapsulation

    /// <summary>
    /// Represents the Dynamic Source Routing Protocol (DSR) with a protocol number of 48.
    /// </summary>
    DSR = 48,             // Dynamic Source Routing Protocol

    /// <summary>
    /// Represents the BNA protocol with a protocol number of 49.
    /// </summary>
    BNA = 49,             // BNA

    /// <summary>
    /// Represents the Encapsulating Security Payload (ESP) protocol with a protocol number of 50.
    /// </summary>
    ESP = 50,             // Encap Security Payload

    /// <summary>
    /// Represents the Authentication Header (AH) protocol with a protocol number of 51.
    /// </summary>
    AH = 51,              // Authentication Header

    /// <summary>
    /// Represents the Integrated Net Layer Security TUBA (I-NLSP) protocol with a protocol number of 52.
    /// </summary>
    I_NLSP = 52,          // Integrated Net Layer Security TUBA

    /// <summary>
    /// Represents the SWIPE protocol with a protocol number of 53.
    /// </summary>
    SWIPE = 53,           // SWIPE

    /// <summary>
    /// Represents the NBMA Address Resolution Protocol (NARP) with a protocol number of 54.
    /// </summary>
    NARP = 54,            // NBMA Address Resolution Protocol

    /// <summary>
    /// Represents the IP Mobility (MOBILE) protocol with a protocol number of 55.
    /// </summary>
    MOBILE = 55,          // IP Mobility

    /// <summary>
    /// Represents the Transport Layer Security Protocol (TLSP) with a protocol number of 56.
    /// </summary>
    TLSP = 56,            // Transport Layer Security Protocol

    /// <summary>
    /// Represents the SKIP protocol with a protocol number of 57.
    /// </summary>
    SKIP = 57,            // SKIP

    /// <summary>
    /// Represents the ICMP for IPv6 (IPv6-ICMP) protocol with a protocol number of 58.
    /// </summary>
    IPv6_ICMP = 58,       // ICMP for IPv6

    /// <summary>
    /// Represents the No Next Header for IPv6 (IPv6-NoNxt) protocol with a protocol number of 59.
    /// </summary>
    IPv6_NoNxt = 59,      // No Next Header for IPv6

    /// <summary>
    /// Represents the Destination Options for IPv6 (IPv6-Opts) protocol with a protocol number of 60.
    /// </summary>
    IPv6_Opts = 60,       // Destination Options for IPv6

    /// <summary>
    /// Represents the CFTP protocol with a protocol number of 62.
    /// </summary>
    CFTP = 62,            // CFTP

    /// <summary>
    /// Represents the SATNET and Backroom EXPAK (SAT-EXPAK) protocol with a protocol number of 64.
    /// </summary>
    SAT_EXPAK = 64,       // SATNET and Backroom EXPAK

    /// <summary>
    /// Represents the Kryptolan protocol with a protocol number of 65.
    /// </summary>
    KRYPTOLAN = 65,       // Kryptolan

    /// <summary>
    /// Represents the MIT Remote Virtual Disk Protocol (RVD) with a protocol number of 66.
    /// </summary>
    RVD = 66,             // MIT Remote Virtual Disk Protocol

    /// <summary>
    /// Represents the Internet Pluribus Packet Core (IPPC) protocol with a protocol number of 67.
    /// </summary>
    IPPC = 67,            // Internet Pluribus Packet Core

    /// <summary>
    /// Represents the SATNET Monitoring (SAT-MON) protocol with a protocol number of 69.
    /// </summary>
    SAT_MON = 69,         // SATNET Monitoring

    /// <summary>
    /// Represents the VISA Protocol with a protocol number of 70.
    /// </summary>
    VISA = 70,            // VISA Protocol

    /// <summary>
    /// Represents the Internet Packet Core Utility (IPCV) protocol with a protocol number of 71.
    /// </summary>
    IPCV = 71,            // Internet Packet Core Utility

    /// <summary>
    /// Represents the Computer Protocol Network Executive (CPNX) protocol with a protocol number of 72.
    /// </summary>
    CPNX = 72,            // Computer Protocol Network Executive

    /// <summary>
    /// Represents the Computer Protocol Heart Beat (CPHB) protocol with a protocol number of 73.
    /// </summary>
    CPHB = 73,            // Computer Protocol Heart Beat

    /// <summary>
    /// Represents the Wang Span Network (WSN) protocol with a protocol number of 74.
    /// </summary>
    WSN = 74,             // Wang Span Network

    /// <summary>
    /// Represents the Packet Video Protocol (PVP) with a protocol number of 75.
    /// </summary>
    PVP = 75,             // Packet Video Protocol

    /// <summary>
    /// Represents the Backroom SATNET Monitoring (BR-SAT-MON) protocol with a protocol number of 76.
    /// </summary>
    BR_SAT_MON = 76,      // Backroom SATNET Monitoring

    /// <summary>
    /// Represents the SUN ND PROTOCOL-Temporary (SUN-ND) with a protocol number of 77.
    /// </summary>
    SUN_ND = 77,          // SUN ND PROTOCOL-Temporary

    /// <summary>
    /// Represents the WIDEBAND Monitoring (WB-MON) protocol with a protocol number of 78.
    /// </summary>
    WB_MON = 78,          // WIDEBAND Monitoring

    /// <summary>
    /// Represents the WIDEBAND EXPAK (WB-EXPAK) protocol with a protocol number of 79.
    /// </summary>
    WB_EXPAK = 79,        // WIDEBAND EXPAK

    /// <summary>
    /// Represents the ISO Internet Protocol (ISO-IP) with a protocol number of 80.
    /// </summary>
    ISO_IP = 80,          // ISO Internet Protocol

    /// <summary>
    /// Represents the Versatile Message Transport Protocol (VMTP) with a protocol number of 81.
    /// </summary>
    VMTP = 81,            // Versatile Message Transport

    /// <summary>
    /// Represents the Secure VMTP protocol with a protocol number of 82.
    /// </summary>
    SECURE_VMTP = 82,     // Secure VMTP

    /// <summary>
    /// Represents the VINES protocol with a protocol number of 83.
    /// </summary>
    VINES = 83,           // VINES

    /// <summary>
    /// Represents the Internet Protocol Traffic Manager (IPTM) protocol with a protocol number of 84.
    /// </summary>
    IPTM = 84,            // Protocol Internet Protocol Traffic Manager

    /// <summary>
    /// Represents the NSFNET-IGP protocol with a protocol number of 85.
    /// </summary>
    NSFNET_IGP = 85,      // NSFNET-IGP

    /// <summary>
    /// Represents the Dissimilar Gateway Protocol (DGP) with a protocol number of 86.
    /// </summary>
    DGP = 86,             // Dissimilar Gateway Protocol

    /// <summary>
    /// Represents the TCF protocol with a protocol number of 87.
    /// </summary>
    TCF = 87,             // TCF

    /// <summary>
    /// Represents the Enhanced Interior Gateway Routing Protocol (EIGRP) with a protocol number of 88.
    /// </summary>
    EIGRP = 88,           // EIGRP

    /// <summary>
    /// Represents the Open Shortest Path First (OSPF) protocol with a protocol number of 89.
    /// </summary>
    OSPF = 89,            // Open Shortest Path First

    /// <summary>
    /// Represents the Sprite RPC Protocol with a protocol number of 90.
    /// </summary>
    Sprite_RPC = 90,      // Sprite RPC Protocol

    /// <summary>
    /// Represents the Locus Address Resolution Protocol (LARP) with a protocol number of 91.
    /// </summary>
    LARP = 91,            // Locus Address Resolution Protocol

    /// <summary>
    /// Represents the Multicast Transport Protocol (MTP) with a protocol number of 92.
    /// </summary>
    MTP = 92,             // Multicast Transport Protocol

    /// <summary>
    /// Represents the AX.25 Frames protocol with a protocol number of 93.
    /// </summary>
    AX_25 = 93,           // AX.25 Frames

    /// <summary>
    /// Represents the IP-within-IP Encapsulation Protocol (IPIP) with a protocol number of 94.
    /// </summary>
    IPIP = 94,            // IP-within-IP Encapsulation Protocol

    /// <summary>
    /// Represents the Mobile Internetworking Control Protocol (MICP) with a protocol number of 95.
    /// </summary>
    MICP = 95,            // Mobile Internetworking Control Pro.

    /// <summary>
    /// Represents the Semaphore Communications Security Protocol (SCC-SP) with a protocol number of 96.
    /// </summary>
    SCC_SP = 96,          // Semaphore Communications Sec. Pro.

    /// <summary>
    /// Represents the Ethernet-within-IP Encapsulation (ETHERIP) protocol with a protocol number of 97.
    /// </summary>
    ETHERIP = 97,         // Ethernet-within-IP Encapsulation

    /// <summary>
    /// Represents the Encapsulation Header (ENCAP) protocol with a protocol number of 98.
    /// </summary>
    ENCAP = 98,           // Encapsulation Header

    /// <summary>
    /// Represents the GMTP protocol with a protocol number of 100.
    /// </summary>
    GMTP = 100,           // GMTP

    /// <summary>
    /// Represents the Ipsilon Flow Management Protocol (IFMP) with a protocol number of 101.
    /// </summary>
    IFMP = 101,           // Ipsilon Flow Management Protocol

    /// <summary>
    /// Represents the PNNI over IP protocol with a protocol number of 102.
    /// </summary>
    PNNI = 102,           // PNNI over IP

    /// <summary>
    /// Represents the Protocol Independent Multicast (PIM) protocol with a protocol number of 103.
    /// </summary>
    PIM = 103,            // Protocol Independent Multicast

    /// <summary>
    /// Represents the ARIS protocol with a protocol number of 104.
    /// </summary>
    ARIS = 104,           // ARIS

    /// <summary>
    /// Represents the SCPS protocol with a protocol number of 105.
    /// </summary>
    SCPS = 105,           // SCPS

    /// <summary>
    /// Represents the QNX protocol with a protocol number of 106.
    /// </summary>
    QNX = 106,            // QNX

    /// <summary>
    /// Represents the Active Networks (A-N) protocol with a protocol number of 107.
    /// </summary>
    A_N = 107,            // Active Networks

    /// <summary>
    /// Represents the IP Payload Compression Protocol (IPComp) with a protocol number of 108.
    /// </summary>
    IPComp = 108,         // IP Payload Compression Protocol

    /// <summary>
    /// Represents the Sitara Networks Protocol (SNP) with a protocol number of 109.
    /// </summary>
    SNP = 109,            // Sitara Networks Protocol

    /// <summary>
    /// Represents the Compaq Peer Protocol with a protocol number of 110.
    /// </summary>
    Compaq_Peer = 110,    // Compaq Peer Protocol

    /// <summary>
    /// Represents the IPX in IP protocol with a protocol number of 111.
    /// </summary>
    IPX_in_IP = 111,      // IPX in IP

    /// <summary>
    /// Represents the Virtual Router Redundancy Protocol (VRRP) with a protocol number of 112.
    /// </summary>
    VRRP = 112,           // Virtual Router Redundancy Protocol

    /// <summary>
    /// Represents the PGM Reliable Transport Protocol (PGM) with a protocol number of 113.
    /// </summary>
    PGM = 113,            // PGM Reliable Transport Protocol

    /// <summary>
    /// Represents the Layer Two Tunneling Protocol (L2TP) with a protocol number of 115.
    /// </summary>
    L2TP = 115,           // Layer Two Tunneling Protocol

    /// <summary>
    /// Represents the D-II Data Exchange (DDX) protocol with a protocol number of 116.
    /// </summary>
    DDX = 116,            // D-II Data Exchange (DDX)

    /// <summary>
    /// Represents the Interactive Agent Transfer Protocol (IATP) with a protocol number of 117.
    /// </summary>
    IATP = 117,           // Interactive Agent Transfer Protocol

    /// <summary>
    /// Represents the Schedule Transfer Protocol (STP) with a protocol number of 118.
    /// </summary>
    STP = 118,            // Schedule Transfer Protocol

    /// <summary>
    /// Represents the SpectraLink Radio Protocol (SRP) with a protocol number of 119.
    /// </summary>
    SRP = 119,            // SpectraLink Radio Protocol

    /// <summary>
    /// Represents the Universal Transport Interface Protocol (UTI) with a protocol number of 120.
    /// </summary>
    UTI = 120,            // Universal Transport Interface Protocol

    /// <summary>
    /// Represents the Simple Message Protocol (SMP) with a protocol number of 121.
    /// </summary>
    SMP = 121,            // Simple Message Protocol

    /// <summary>
    /// Represents the Simple Multicast Protocol (SM) with a protocol number of 122.
    /// </summary>
    SM = 122,             // Simple Multicast Protocol

    /// <summary>
    /// Represents the Performance Transparency Protocol (PTP) with a protocol number of 123.
    /// </summary>
    PTP = 123,            // Performance Transparency Protocol

    /// <summary>
    /// Represents the ISIS over IPv4 protocol with a protocol number of 124.
    /// </summary>
    ISIS_over_IPv4 = 124, // ISIS over IPv4

    /// <summary>
    /// Represents the Flexible Intra-AS Routing Environment (FIRE) protocol with a protocol number of 125.
    /// </summary>
    FIRE = 125,           // Flexible Intra-AS Routing Environment

    /// <summary>
    /// Represents the Combat Radio Transport Protocol (CRTP) with a protocol number of 126.
    /// </summary>
    CRTP = 126,           // Combat Radio Transport Protocol

    /// <summary>
    /// Represents the Combat Radio User Datagram Protocol (CRUDP) with a protocol number of 127.
    /// </summary>
    CRUDP = 127,          // Combat Radio User Datagram

    /// <summary>
    /// Represents the SSCOPMCE protocol with a protocol number of 128.
    /// </summary>
    SSCOPMCE = 128,       // SSCOPMCE

    /// <summary>
    /// Represents the IPLT protocol with a protocol number of 129.
    /// </summary>
    IPLT = 129,           // IPLT

    /// <summary>
    /// Represents the Secure Packet Shield (SPS) protocol with a protocol number of 130.
    /// </summary>
    SPS = 130,            // Secure Packet Shield

    /// <summary>
    /// Represents the Private IP Encapsulation within IP (PIPE) protocol with a protocol number of 131.
    /// </summary>
    PIPE = 131,           // Private IP Encapsulation within IP

    /// <summary>
    /// Represents the Stream Control Transmission Protocol (SCTP) with a protocol number of 132.
    /// </summary>
    SCTP = 132,           // Stream Control Transmission Protocol

    /// <summary>
    /// Represents the Fibre Channel (FC) protocol with a protocol number of 133.
    /// </summary>
    FC = 133,             // Fibre Channel

    /// <summary>
    /// Represents the RSVP-E2E-IGNORE protocol with a protocol number of 134.
    /// </summary>
    RSVP_E2E_IGNORE = 134,// RSVP-E2E-IGNORE

    /// <summary>
    /// Represents the Mobility Header protocol with a protocol number of 135.
    /// </summary>
    Mobility_Header = 135,// Mobility Header

    /// <summary>
    /// Represents the UDP-Lite protocol with a protocol number of 136.
    /// </summary>
    UDPLite = 136,        // UDP-Lite

    /// <summary>
    /// Represents the MPLS-in-IP protocol with a protocol number of 137.
    /// </summary>
    MPLS_in_IP = 137,     // MPLS-in-IP

    /// <summary>
    /// Represents the MANET Protocols (manet) with a protocol number of 138.
    /// </summary>
    manet = 138,          // MANET Protocols

    /// <summary>
    /// Represents the Host Identity Protocol (HIP) with a protocol number of 139.
    /// </summary>
    HIP = 139,            // Host Identity Protocol

    /// <summary>
    /// Represents the Shim6 Protocol with a protocol number of 140.
    /// </summary>
    Shim6 = 140,          // Shim6 Protocol

    /// <summary>
    /// Represents the Wrapped Encapsulating Security Payload (WESP) protocol with a protocol number of 141.
    /// </summary>
    WESP = 141,           // Wrapped Encapsulating Security Payload

    /// <summary>
    /// Represents the Robust Header Compression (ROHC) protocol with a protocol number of 142.
    /// </summary>
    ROHC = 142,           // Robust Header Compression

    /// <summary>
    /// Represents the Ethernet protocol with a protocol number of 143.
    /// </summary>
    Ethernet = 143,       // Ethernet

    /// <summary>
    /// Represents the Reserved protocol number 255.
    /// </summary>
    Reserved_255 = 255,    // Reserved
}
