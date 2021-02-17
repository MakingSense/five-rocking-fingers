import * as React from 'react'
import { Typography, Box } from '@material-ui/core'
import { createMuiTheme } from '@material-ui/core/styles'
import { ThemeProvider } from '@material-ui/styles'
import { green, red } from '@material-ui/core/colors'

const theme = createMuiTheme({
  palette: {
    primary: green,
    secondary: red,
  },
})

const ArtifactsTotalPrice = (props: { totalPrice: string; projectBudget: number }) => {
  const { totalPrice, projectBudget } = props

  const isOnProjectBudget = (): boolean => {
    return parseInt(totalPrice) <= projectBudget
  }

  return (
    <>
      <tr>
        <td colSpan={5}>
          <ThemeProvider theme={theme}>
            <Typography
              color={isOnProjectBudget() ? 'primary' : 'secondary'}
              component={'span'}
              variant="h5"
              gutterBottom
            >
              <Box fontWeight="fontWeightBold" textAlign="right" m={1}>
                Precio Total: ${totalPrice} /m
              </Box>
            </Typography>
          </ThemeProvider>
        </td>
      </tr>
    </>
  )
}

export default ArtifactsTotalPrice
