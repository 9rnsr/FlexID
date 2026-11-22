[include:common_35-Br_parameter.inc]

[parameter]
  # ICRP Publ.151 p.198 Table 21.3 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Br}/fA    = 0.99 # ≒1          # ICRP Publ.151 p.198 Table 21.2 / Unspecified
  ${Br}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.130 p.62 Para.92
# ICRP Publ.151 p.198 Table 21.2
  ET1-F                 $(      fr  *                  0% )
  ET2-F                 $(      fr  * (100% - 0.2%) * 20% )
  ETseq-F               $(      fr  *         0.2%  * 20% )
  BB-F                  $(      fr  * (100% - 0.2%) * 10% )
  BBseq-F               $(      fr  *         0.2%  * 10% )
  bb-F                  $(      fr  * (100% - 0.2%) * 20% )
  bbseq-F               $(      fr  *         0.2%  * 20% )
  ALV-F                 $(      fr  *                 50% )

  ET1-S                 $( (1 - fr) *                  0% )
  ET2-S                 $( (1 - fr) * (100% - 0.2%) * 20% )
  ETseq-S               $( (1 - fr) *         0.2%  * 20% )
  BB-S                  $( (1 - fr) * (100% - 0.2%) * 10% )
  BBseq-S               $( (1 - fr) *         0.2%  * 10% )
  bb-S                  $( (1 - fr) * (100% - 0.2%) * 20% )
  bbseq-S               $( (1 - fr) *         0.2%  * 20% )
  ALV-S                 $( (1 - fr) *                 50% )

  Environment           $(                             0% )


[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_36-Kr.inc]
[include:element_35-Br.inc]
[include:element_34-Se.inc]
