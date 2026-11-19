[include:common_20-Ca_parameter.inc]

[parameter]
  # ICRP Publ.134 p.118 Table 6.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Ca}/fA    = 0.08
  ${Ca}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_20-Ca.inc]
[include:element_21-Sc.inc]
