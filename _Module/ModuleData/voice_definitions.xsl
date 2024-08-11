<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="voice_definition[@name='male_01']/@min_pitch_multiplier">
    <xsl:attribute name="min_pitch_multiplier">0.8</xsl:attribute>
  </xsl:template>
  <xsl:template match="voice_definition[@name='male_01']/@max_pitch_multiplier">
    <xsl:attribute name="max_pitch_multiplier">0.85</xsl:attribute>
  </xsl:template>

  <xsl:template match="voice_definition[@name='male_02']/@min_pitch_multiplier">
    <xsl:attribute name="min_pitch_multiplier">0.8</xsl:attribute>
  </xsl:template>
  <xsl:template match="voice_definition[@name='male_02']/@max_pitch_multiplier">
    <xsl:attribute name="max_pitch_multiplier">0.85</xsl:attribute>
  </xsl:template>

  <xsl:template match="voice_definition[@name='male_03']/@min_pitch_multiplier">
    <xsl:attribute name="min_pitch_multiplier">0.8</xsl:attribute>
  </xsl:template>
  <xsl:template match="voice_definition[@name='male_03']/@max_pitch_multiplier">
    <xsl:attribute name="max_pitch_multiplier">0.85</xsl:attribute>
  </xsl:template>

  <xsl:template match="voice_definition[@name='male_04']/@min_pitch_multiplier">
    <xsl:attribute name="min_pitch_multiplier">0.8</xsl:attribute>
  </xsl:template>
  <xsl:template match="voice_definition[@name='male_04']/@max_pitch_multiplier">
    <xsl:attribute name="max_pitch_multiplier">0.85</xsl:attribute>
  </xsl:template>

  <xsl:template match="voice_definition[@name='male_05']/@min_pitch_multiplier">
    <xsl:attribute name="min_pitch_multiplier">0.75</xsl:attribute>
  </xsl:template>
  <xsl:template match="voice_definition[@name='male_05']/@max_pitch_multiplier">
    <xsl:attribute name="max_pitch_multiplier">0.8</xsl:attribute>
  </xsl:template>

  <xsl:template match="voice_definition[@name='male_06']/@min_pitch_multiplier">
    <xsl:attribute name="min_pitch_multiplier">0.8</xsl:attribute>
  </xsl:template>
  <xsl:template match="voice_definition[@name='male_06']/@max_pitch_multiplier">
    <xsl:attribute name="max_pitch_multiplier">0.85</xsl:attribute>
  </xsl:template>

  <xsl:template match="voice_definition[@name='male_07']/@min_pitch_multiplier">
    <xsl:attribute name="min_pitch_multiplier">0.8</xsl:attribute>
  </xsl:template>
  <xsl:template match="voice_definition[@name='male_07']/@max_pitch_multiplier">
    <xsl:attribute name="max_pitch_multiplier">0.85</xsl:attribute>
  </xsl:template>

  <xsl:template match="voice_definition[@name='male_08']/@min_pitch_multiplier">
    <xsl:attribute name="min_pitch_multiplier">0.75</xsl:attribute>
  </xsl:template>
  <xsl:template match="voice_definition[@name='male_08']/@max_pitch_multiplier">
    <xsl:attribute name="max_pitch_multiplier">0.8</xsl:attribute>
  </xsl:template>
</xsl:stylesheet>
