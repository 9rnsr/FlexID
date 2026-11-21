[include:common_21-Sc_parameter.inc]

[parameter]
  # ICRP Publ.151 p.88 Table 10.2 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${Sc}/fA    = 1E-5
  ${Sc}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_21-Sc.inc]
