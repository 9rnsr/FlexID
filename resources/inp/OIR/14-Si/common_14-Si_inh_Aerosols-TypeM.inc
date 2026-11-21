[include:common_14-Si_parameter.inc]

[parameter]
  # ICRP Publ.151 p.67 Table 7.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${Si}/fA    = 0.1
  ${Si}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_15-P.inc]
[include:element_14-Si.inc]
