[include:common_19-K_parameter.inc]

[parameter]
  # ICRP Publ.151 p.79 Table 9.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${K}/fA    = 0.99 # ≒1
  ${K}/fA_Re = fA_MaxValueOfIng     # footnote§
$else
  ${*}/fA    = fr * fA_MaxValueOfIng
  ${*}/fA_Re =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_20-Ca.inc]
[include:element_19-K.inc]
