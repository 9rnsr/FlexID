[include:common_92-U_parameter.inc]

[parameter]
  # ICRP Publ.137 p.409 Table 15.2 / Inhaled particulate materials, Intermediate Type F/M

  $fr = 0.8
  $sr = 1
  $ss = 0.01
  $fb = 0.0         # footnote*
  $sb = 0           # footnote*

$if parent
  ${U}/fA      = 0.016
  ${U}/fA_Re   = fA_MaxValueOfIng   # footnote††
$else
  ${^Rn}/fA    = fr * fA_MaxValueOfIng
  ${^Rn}/fA_Re =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh-Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_94-Pu.inc]
[include:element_93-Np.inc]
[include:element_92-U.inc]
[include:element_91-Pa.inc]
[include:element_90-Th.inc]
[include:element_89-Ac.inc]
[include:element_88-Ra.inc]
[include:element_87-Fr.inc]
[include:element_86-Rn.inc]
[include:element_85-At.inc]
[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
