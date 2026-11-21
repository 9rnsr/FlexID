[include:common_24-Cr_parameter.inc]

[parameter]
  # ICRP Publ.151 p.109 Table 13.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${Cr}/fA    = 0.002
  ${Cr}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_24-Cr.inc]
[include:element_23-V.inc]
