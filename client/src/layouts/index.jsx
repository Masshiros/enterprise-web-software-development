import React from 'react'
import Header from './partials/Header'
import Footer from './partials/Footer'

export default function GeneralLayout({ children }) {
  return (
    <>
      <Header></Header>
      <div className='flex flex-col min-h-screen mt-5'>{children}</div>
      <Footer></Footer>
    </>
  )
}
