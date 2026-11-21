[include:common_23-V_parameter.inc]

[parameter]
  # ICRP Publ.151 p.103 Table 12.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${V}/fA    = 0.2
  ${V}/fA_Re = fA_MaxValueOfIng     # footnote§
$else
  ${*}/fA    = fr * fA_MaxValueOfIng
  ${*}/fA_Re =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_23-V.inc]
