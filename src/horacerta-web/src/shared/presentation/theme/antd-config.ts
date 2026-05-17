import { theme as antdTheme, type ThemeConfig } from 'antd';
import { palette, type ThemeMode } from './tokens';

export function buildAntdTheme(mode: ThemeMode): ThemeConfig {
  const isDark = mode === 'dark';

  return {
    algorithm: isDark ? antdTheme.darkAlgorithm : antdTheme.defaultAlgorithm,
    token: {
      colorPrimary: palette.primary,
      colorInfo: palette.primary,
      colorSuccess: palette.primary,
      colorLink: palette.primary,
      colorLinkHover: palette.accent,
      borderRadius: 12,
      borderRadiusLG: 16,
      fontFamily: 'var(--hc-font-family)',
      colorBgLayout: isDark ? palette.dark.bg : palette.light.bg,
      colorBgContainer: isDark ? palette.dark.surface : palette.light.surface,
      colorBgElevated: isDark ? palette.dark.surfaceElevated : palette.light.surface,
      colorText: isDark ? palette.dark.text : palette.light.text,
      colorTextSecondary: isDark ? palette.dark.textMuted : palette.light.textMuted,
      colorBorder: isDark ? palette.dark.border : palette.light.border,
      colorBorderSecondary: isDark ? palette.dark.border : palette.light.border,
    },
    components: {
      Layout: {
        bodyBg: isDark ? palette.dark.bg : palette.light.bg,
        headerBg: isDark ? palette.dark.surface : palette.light.surface,
        siderBg: palette.light.header,
        triggerBg: palette.dark.surfaceElevated,
      },
      Menu: {
        darkItemBg: palette.light.header,
        darkSubMenuItemBg: '#1E293B',
        darkItemSelectedBg: palette.primary,
        darkItemSelectedColor: '#FFFFFF',
        itemSelectedColor: palette.primary,
        itemSelectedBg: isDark ? 'rgba(16, 185, 129, 0.15)' : 'rgba(16, 185, 129, 0.1)',
      },
      Card: {
        borderRadiusLG: 16,
        paddingLG: 24,
      },
      Button: {
        primaryShadow: isDark
          ? '0 4px 14px rgba(52, 211, 153, 0.35)'
          : '0 4px 14px rgba(16, 185, 129, 0.28)',
        borderRadius: 10,
      },
      Table: {
        borderRadius: 12,
        headerBg: isDark ? palette.dark.surfaceElevated : palette.light.surfaceMuted,
      },
      Modal: {
        borderRadiusLG: 16,
      },
      Steps: {
        colorPrimary: palette.primary,
      },
      Rate: {
        starColor: palette.accent,
      },
    },
  };
}
