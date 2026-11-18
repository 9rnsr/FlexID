[include:common_53-I_parameter.inc]

[parameter]
  # ICRP Publ.137 p.97 Table 5.3 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 100
  $ss =   0
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${I}/fA    = 0.99 # ≒1           # ICRP Publ.137 p.96 Table 5.1
  ${I}/fA_Re = fA_MaxValueOfIng     # footnote¶
$else
  ${*}/fA    = fr * fA_MaxValueOfIng
  ${*}/fA_Re =      fA_MaxValueOfIng
$endif


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.130 p.62 Para.92
# ICRP Publ.137 p.96 Table 5.2
  ET1-F                 $(      fr  *                  0% )
  ET2-F                 $(      fr  * (100% - 0.2%) * 50% )
  ETseq-F               $(      fr  *         0.2%  * 50% )
  BB-F                  $(      fr  * (100% - 0.2%) * 50% )
  BBseq-F               $(      fr  *         0.2%  * 50% )
  bb-F                  $(      fr  * (100% - 0.2%) *  0% )
  bbseq-F               $(      fr  *         0.2%  *  0% )
  ALV-F                 $(      fr  *                  0% )

  ET1-S                 $( (1 - fr) *                  0% )
  ET2-S                 $( (1 - fr) * (100% - 0.2%) * 50% )
  ETseq-S               $( (1 - fr) *         0.2%  * 50% )
  BB-S                  $( (1 - fr) * (100% - 0.2%) * 50% )
  BBseq-S               $( (1 - fr) *         0.2%  * 50% )
  bb-S                  $( (1 - fr) * (100% - 0.2%) *  0% )
  bbseq-S               $( (1 - fr) *         0.2%  *  0% )
  ALV-S                 $( (1 - fr) *                  0% )

  Environment           $(                             0% )


[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_54-Xe.inc]
[include:element_53-I.inc]
[include:element_52-Te.inc]
[include:element_51-Sb.inc]
