[include:common_35-Br_parameter.inc]

[parameter]
  # ICRP Publ.151 p.198 Table 21.3 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Br}/fA    = 0.99 # ≒1
  ${Br}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_36-Kr.inc]
[include:element_35-Br.inc]
[include:element_34-Se.inc]
