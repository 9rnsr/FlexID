[include:common_40-Zr_parameter.inc]

[parameter]
  # ICRP Publ.134 p.264 Table 12.2 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Zr}/fA    = 2E-5
  ${Zr}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_41-Nb.inc]
[include:element_40-Zr.inc]
[include:element_39-Y.inc]
[include:element_38-Sr.inc]
