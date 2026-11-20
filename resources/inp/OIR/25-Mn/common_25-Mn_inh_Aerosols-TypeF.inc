[include:common_25-Mn_parameter.inc]

[parameter]
  # ICRP Publ.151 p.119 Table 14.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Mn}/fA    = 0.05
  ${Mn}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_25-Mn.inc]
[include:element_24-Cr.inc]
