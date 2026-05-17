'use client';

import Link from 'next/link';
import { Typography } from 'antd';
import { ClockCircleOutlined } from '@ant-design/icons';

type AppBrandProps = {
  href?: string;
  light?: boolean;
  size?: 'sm' | 'md' | 'lg';
};

export function AppBrand({ href = '/', light = false, size = 'md' }: AppBrandProps) {
  const level = size === 'lg' ? 2 : size === 'sm' ? 5 : 4;

  const content = (
    <span className="hc-brand">
      <ClockCircleOutlined className="hc-brand__icon" />
      <Typography.Title level={level} className="hc-brand__text" style={{ margin: 0 }}>
        HoraCerta
      </Typography.Title>
    </span>
  );

  if (href) {
    return (
      <Link href={href} className={light ? 'hc-brand-link hc-brand-link--light' : 'hc-brand-link'}>
        {content}
      </Link>
    );
  }

  return content;
}
