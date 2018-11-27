# SnmpAgent
Monitoring of Devices using SNMP

We start building one of the first multiplatform apps usingXamarin and #SNMP library, to allow devices configuration management and health monitoring in real time using Simple Networking Management Protocol (SNMP). Using this we will able to monitor various aspects of the device health like: uptime, memory usage, ping latency and number of processes and watch real time updates using popular Network Management Systems  (NMS) like Cacti and Nagios. We have also implemented SNMPv3 which incorporates authentication and encryption.

## Usage
Using SNMP GET request, one network manager system can request for value of all or a only particular OID (SNMP v1, v2c & v3). Using SNMP GETNEXT, the manager can ask the value next to that of the current OID (SNMP v1, v2c & v3). Using SNMP SET the manager can set the value of a particular OID (SNMP v1, v2c & v3).

SNMP v1 & v2c Request Comunity => "public"

SNMP v3 Request Credentials:
Privacy Provider noAuthNoPriv user => "neither"
Privacy Provider authPriv =>
MD5 Auth Password: "authentication"
DES Privacy Password: "privacyphrase"
AES Privacy Password: "privacyphrase"
