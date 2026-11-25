[include:common_80-Hg_parameter.inc]

[parameter]
  # ICRP Publ.151 p.327 Table 36.3 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0.24                # footnote*
  $sb = 2.1                 # footnote*

$if parent
  ${Hg}/fA    = 0.02
  ${Hg}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_80-Hg_Divalent.inc]
[include:element_79-Au.inc]
[include:element_78-Pt.inc]
[include:element_76-Os.inc]
