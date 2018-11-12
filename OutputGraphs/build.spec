# -*- mode: python -*-

block_cipher = None

a = Analysis(['OutputGraphs.py'],
             pathex=['C:\\Users\\Rylan\\source\\repos\\\SequenceAnalysis\\OutputGraphs'],
             binaries=[],
             datas=[('C:\\Program Files (x86)\\Microsoft Visual Studio\\Shared\\Python36_64\\Lib\\site-packages\\plotly\\package_data\\*', 'plotly\\package_data')],
             hiddenimports=[],
             hookspath=[],
             runtime_hooks=[],
             excludes=[],
             win_no_prefer_redirects=False,
             win_private_assemblies=False,
             cipher=block_cipher)
pyz = PYZ(a.pure, a.zipped_data,
             cipher=block_cipher)
exe = EXE(pyz,
          a.scripts,
          a.binaries,
          a.zipfiles,
          a.datas,
          name='BuildGraphs',
          debug=False,
          strip=False,
          upx=True,
          runtime_tmpdir=None,
          console=True )