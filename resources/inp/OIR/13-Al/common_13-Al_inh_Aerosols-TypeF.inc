[include:common_13-Al_parameter.inc]

[parameter]
  # ICRP Publ.151 p.56 Table 6.6 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${Al}/fA    = 0.003
  ${Al}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_13-Al.inc]
