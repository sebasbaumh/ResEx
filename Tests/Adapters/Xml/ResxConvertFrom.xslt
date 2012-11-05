<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>
    <xsl:template match="root">
        <VIPTunnel>
            <strings>
                <xsl:for-each select="data">
                    <xsl:element name="{@name}">
                        <xsl:attribute name="text">
                            <xsl:value-of select="value"/>
                        </xsl:attribute>
                    </xsl:element>
                </xsl:for-each>
            </strings>
        </VIPTunnel>
    </xsl:template>
</xsl:stylesheet>