import Search from '@/components/Search'
import { Button } from '@/components/ui/button'
import AdminLayout from '@/layouts/AdminLayout'
import { ArrowDown10Icon, Plus } from 'lucide-react'
import React, { useEffect, useState } from 'react'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger
} from '@/components/ui/dropdown-menu'

import Article from '@/components/article'
import { createSearchParams, useNavigate } from 'react-router-dom'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import useParamsVariables from '@/hooks/useParams'
import { isUndefined, omitBy, omit, debounce } from 'lodash'
import { Icon } from '@iconify/react'
import { Checkbox } from '@/components/ui/checkbox'
import { ArrowUpDown } from 'lucide-react'

import Spinner from '@/components/Spinner'
import { MC_OPTIONS, STUDENT_OPTIONS } from '@/constant/menuSidebar'
import { Contributions } from '@/services/coodinator'
import { CustomTable } from '@/components/CustomTable'
import { formatDate } from '@/utils/helper'

import Swal from 'sweetalert2'
import { toast } from 'react-toastify'

export default function SettingGAC() {
  const [position, setPosition] = React.useState('')
  const navigate = useNavigate()
  const queryParams = useParamsVariables()
  const [inputValue, setInputValue] = useState('')
  const columns = [
    {
      id: 'select',
      header: ({ table }) => (
        <Checkbox
          checked={
            table.getIsAllPageRowsSelected() ||
            (table.getIsSomePageRowsSelected() && 'indeterminate')
          }
          onCheckedChange={(value) => table.toggleAllPageRowsSelected(!!value)}
          aria-label='Select all'
          className='mx-4'
        />
      ),
      cell: ({ row }) => (
        <Checkbox
          checked={row.getIsSelected()}
          onCheckedChange={(value) => row.toggleSelected(!!value)}
          aria-label='Select row'
        />
      ),
      enableSorting: false,
      enableHiding: false
    },
    {
      accessorKey: 'title',
      header: ({ column }) => {
        return (
          <Button
            variant='ghost'
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Contribution Title
            <ArrowUpDown className='w-4 h-4 ml-2' />
          </Button>
        )
      }
    },
    {
      accessorKey: 'shortDescription',
      header: ({ column }) => {
        return (
          <Button
            variant='ghost'
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Short Description
            <ArrowUpDown className='w-4 h-4 ml-2' />
          </Button>
        )
      }
    },
    {
      accessorKey: `publicDate`,
      header: ({ column }) => {
        return (
          <Button
            variant='ghost'
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Date
            <ArrowUpDown className='w-4 h-4 ml-2' />
          </Button>
        )
      }
    },
    {
      accessorKey: 'files.length',
      header: ({ column }) => {
        return (
          <Button
            variant='ghost'
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Files
            <ArrowUpDown className='w-4 h-4 ml-2' />
          </Button>
        )
      }
    }
  ]
  const queryConfig = omitBy(
    {
      pageindex: queryParams.pageindex || '1',
      facultyname: queryParams.facultyname,
      status: 'APPROVE',
      keyword: queryParams.keyword,
      name: queryParams.name,
      year: queryParams.year,
      pagesize: queryParams.pagesize || '4'
    },
    isUndefined
  )
  const { data, isLoading } = useQuery({
    queryKey: ['mc-contributions', queryConfig],
    queryFn: (_) => Contributions.MCContribution(queryConfig)
  })

  const handleInputChange = debounce((value) => {
    if (!value) {
      return navigate({
        pathname: '/coodinator-manage/setting-guest',
        search: createSearchParams(
          omit({ ...queryConfig }, ['keyword'])
        ).toString()
      })
    }

    navigate({
      pathname: '/coodinator-manage/setting-guest',
      search: createSearchParams(
        omitBy(
          {
            ...queryConfig,
            keyword: value
          },
          (value, key) =>
            key === 'pageindex' || key === 'pagesize' || isUndefined(value)
        )
      ).toString()
    })
  }, 300)
  const { mutate } = useMutation({
    mutationFn: (body) => Contributions.MCAllowGuest(body)
  })
  let currentData = data && data?.data?.responseData
  let results = currentData?.results?.map((item) => ({ ...item, publicDate: formatDate(item.publicDate) }))
  const [selectedRow, setSelectedRow] = useState({})
  const handleApproveArticle = () => {
    let ids = []
    selectedRow && selectedRow?.length > 0 && selectedRow.forEach((item) => {
      ids.push(item.id)
    });

    Swal.fire({
      title: "Are you sure?",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, Approve it!"
    }).then((result) => {
      if (result.isConfirmed) {
        mutate({ ids }, {
          onSuccess() {
            toast.success("Update Contribution for Guest Successfully!")
          },
          onError(err) {
            console.log(err)
          }
        })
      }
    });
  }
  return (
    <AdminLayout links={MC_OPTIONS}>
      <div className='flex flex-wrap items-center gap-3 my-5'>
        <div
          className={`flex items-center px-5 py-4 border rounded-lg gap-x-2 w-[50vw]`}
        >
          <Icon
            icon='ic:outline-search'
            className='flex-shrink-0 w-6 h-6 text-slate-700'
          ></Icon>
          <input
            type='text'
            className='flex-1 border-none outline-none'
            placeholder="What you're looking for ?"
            defaultValue={queryParams['keyword']}
            onChange={(e) => {
              setInputValue(e.target.value)
              handleInputChange(e.target.value)
            }}
          />
        </div>
        {selectedRow?.length > 0 && <Button onClick={handleApproveArticle}>Add selection</Button>}
      </div>
      {!isLoading && (
        <>
          <CustomTable
            columns={columns}
            data={results}
            path={'/coodinator-manage/setting-guest'}
            queryConfig={queryConfig}
            pageCount={currentData?.pageCount}
            selectedRows={setSelectedRow}
          ></CustomTable>
        </>
      )}
      {isLoading && (
        <div className='flex justify-center min-h-screen mt-10'>
          <Spinner></Spinner>
        </div>
      )}

      {!isLoading && results?.length < 0 && (
        <div className='my-10 text-3xl font-semibold text-center '>No Data</div>
      )}
    </AdminLayout>
  )
}
