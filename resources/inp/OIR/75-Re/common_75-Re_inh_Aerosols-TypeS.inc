[include:common_75-Re_parameter.inc]

[parameter]
  # ICRP Publ.151 p.288 Table 32.2 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Re}/fA    = 0.009
  ${Re}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_76-Os.inc]
[include:element_75-Re.inc]
[include:element_74-W.inc]
[include:element_73-Ta.inc]
