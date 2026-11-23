[include:common_48-Cd_parameter.inc]

[parameter]
  # ICRP Publ.151 p.238 Table 26.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Cd}/fA    = 0.05
  ${Cd}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_50-Sn.inc]
[include:element_49-In.inc]
[include:element_48-Cd.inc]
[include:element_47-Ag.inc]
