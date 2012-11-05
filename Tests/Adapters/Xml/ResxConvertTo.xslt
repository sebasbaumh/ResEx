<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>
    <xsl:template match="VIPTunnel/strings">
        <root>
            <resheader name="resmimetype">
                <value>text/microsoft-resx</value>
            </resheader>
            <resheader name="version">
                <value>2.0</value>
            </resheader>
            <resheader name="reader">
                <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
            </resheader>
            <resheader name="writer">
                <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
            </resheader>
            <xsl:for-each select="@* | node()">
            <xsl:element name="data">
				<xsl:attribute name="name"><xsl:value-of select="name()"/></xsl:attribute>
				<xsl:attribute name="xml:space">preserve</xsl:attribute>
				<value><xsl:value-of select="@text"/></value>
            </xsl:element>
            </xsl:for-each>
        </root>
    </xsl:template>
</xsl:stylesheet>
