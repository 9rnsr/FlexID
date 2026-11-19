[include:common_76-Os_parameter.inc]

[parameter]
  # ICRP Publ.151 p.298 Table 33.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${Os}/fA    = 0.002
  ${Os}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_77-Ir.inc]
[include:element_76-Os.inc]
[include:element_75-Re.inc]
[include:element_74-W.inc]
